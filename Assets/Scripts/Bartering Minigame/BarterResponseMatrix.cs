using UnityEngine;
using System.Collections.Generic;
using System.Linq;




#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New BarterResponseMatrix", menuName = "Bartering/BarterResponseMatrix")]
public class BarterResponseMatrix : ScriptableObject
{
    // Parameters and Publics =====================================================================

    public enum State {
        POSITIVE,
        NEUTRAL,
        NEGATIVE
    }

    [SerializeField, Tooltip("The cards that this opponent can play.")]
    private PlayingCard[] oppCards = new PlayingCard[0];
    public PlayingCard[] OppCards => oppCards;

    [Tooltip("An ID used for debug purposes.")]
    public string id;
    [Tooltip("The default state returned whenever the player plays a card not in OppCards.")]
    public State defaultState = State.NEUTRAL;
    public Dictionary<PlayingCard, Dictionary<PlayingCard, State>> oppPlayerToMatch;

    // Misc Internal Variables ====================================================================



    // Initializers ===============================================================================

    private void OnEnable()
    {
        UpdateOppCards();
    }

    /// <summary>
    /// Populate our internal response lookup table. MUST be called before GetResponse() is used.
    /// </summary>
    public void Initialize()
    {
        return;
    }

    /// <summary>
    /// Takes two playing cards, returns the opponent's reaction to the matchup. 
    /// </summary>
    /// <param name="oppCard">PlayingCard - the opponent's card.</param>
    /// <param name="playerCard">PlayingCard - the player's card.</param>
    /// <returns>State - State.POSITIVE, State.NEUTRAL, or State.NEGATIVE.</returns>
    public State GetMatch(PlayingCard oppCard, PlayingCard playerCard)
    {
        if (!oppCards.Contains(oppCard)) {
            Debug.LogError($"BarterResponseMatrix Error: GetMatch failed. Supplied opponent card "
                         + $"(id {oppCard}) was not in oppPlayerToMatch dictionary.");
        }

        if (!oppPlayerToMatch[oppCard].ContainsKey(playerCard)) {
            return defaultState;
        }

        // Else, our indexing is valid!
        return oppPlayerToMatch[oppCard][playerCard];
    }

    public void SetMatch(PlayingCard oppCard, PlayingCard playerCard, State match)
    {
        oppPlayerToMatch[oppCard][playerCard] = match;
    }

    public void RemoveDuplicateOppCards()
    {
        oppCards = oppCards.Distinct().Where(x => x != null).ToArray();
    }

    public void UpdateOppCards()
    {
        oppPlayerToMatch ??= new();

        // Remove missing cards.
        foreach (PlayingCard oldCard in oppPlayerToMatch.Keys.ToList()) {
            if (!oppCards.Contains(oldCard)) {
                // Ignore this card when played by the opp...
                oppPlayerToMatch.Remove(oldCard);
                // And ignore this card when played by the player...
                foreach (PlayingCard remainingCard in oppPlayerToMatch.Keys) {
                    oppPlayerToMatch[remainingCard].Remove(oldCard);
                }
            }
        }

        // Add new cards.
        foreach (PlayingCard newCard in oppCards) {
            // If the card is new, initialize it.
            if (!oppPlayerToMatch.ContainsKey(newCard)) {
                // Pay attention when this card is played by the opp...
                oppPlayerToMatch[newCard] = new();
                // And when this card is played by the player.
                foreach (PlayingCard existingCard in oppPlayerToMatch.Keys) {
                    oppPlayerToMatch[existingCard][newCard] = State.NEUTRAL;
                    Debug.Log($"Set card at ({existingCard}, {newCard}) to NEUTRAL");

                    if (!newCard.Matches(existingCard)) {
                        oppPlayerToMatch[newCard][existingCard] = State.NEUTRAL;
                        Debug.Log($"Set card at ({newCard}, {existingCard}) to NEUTRAL");
                    }
                }
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BarterResponseMatrix))]
public class EditorSpawnTemplate : Editor
{
    private BarterResponseMatrix _matrix;
    // Check, empty character, or cross
    private string[] _stateOpts = new []{"✓", "‎", "✗"};
    private int[,] _choiceIndex;
    private int _lastCount;
    private static uint _idLength = 3;
    private string[] _oppCardNames;

    private void OnEnable()
    {
        _matrix = (BarterResponseMatrix)target;

        _matrix.RemoveDuplicateOppCards();
        _matrix.UpdateOppCards();

        _lastCount = _matrix.OppCards.Length;

        RegenerateTableHeaders();

        _choiceIndex = new int[_lastCount,_lastCount];
        for (int r=0; r < _lastCount; r++) {
            for (int c=0; c < _lastCount; c++) {
                PlayingCard oppCard = _matrix.OppCards[c];
                PlayingCard playerCard = _matrix.OppCards[r];
                _choiceIndex[r,c] = (int)_matrix.GetMatch(oppCard, playerCard);
            }
        }
    }

    public override void OnInspectorGUI()
    {
        if (!_matrix) return;

        serializedObject.Update();

        if (_matrix.OppCards.Length < _lastCount) {
            UpdateMatrix();
        }

        if (GUILayout.Button("Update OppCards in Matrix")) {
            UpdateMatrix();
        }

        EditorGUILayout.BeginHorizontal();
            EditorGUIUtility.labelWidth = 1;
            EditorGUILayout.LabelField("Opponent >", EditorStyles.miniLabel);
            for (int c = 0; c < _lastCount; c++) {
                EditorGUILayout.LabelField(GetShortId(c), EditorStyles.boldLabel);
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Player v", EditorStyles.miniLabel);

        for (int r = 0; r < _lastCount; r++) {
            EditorGUILayout.BeginHorizontal();

                EditorGUIUtility.labelWidth = 1;
                EditorGUILayout.LabelField(GetShortId(r), EditorStyles.boldLabel);

                for (int c = 0; c < _lastCount; c++) {

                    Color color = (BarterResponseMatrix.State)_choiceIndex[r,c] switch {
                        BarterResponseMatrix.State.POSITIVE => Color.green,
                        BarterResponseMatrix.State.NEGATIVE => Color.red,
                        _ => Color.white
                    };

                    GUI.backgroundColor = color;
                    int newIndex = EditorGUILayout.Popup(_choiceIndex[r,c], _stateOpts);
                    // If changed, write back an update.
                    if (newIndex != _choiceIndex[r,c]) {
                        _choiceIndex[r,c] = newIndex;
                        _matrix.SetMatch(_matrix.OppCards[c], _matrix.OppCards[r], 
                                         (BarterResponseMatrix.State)newIndex);
                    }   
                }

            EditorGUILayout.EndHorizontal();
            GUI.backgroundColor = Color.white;
        }

        // Reset label width and GUI color.
        EditorGUIUtility.labelWidth = 0;
        GUI.backgroundColor = Color.white;

        serializedObject.ApplyModifiedProperties();

        base.OnInspectorGUI();
    }

    private void UpdateMatrix()
    {
        _matrix.RemoveDuplicateOppCards();
        _matrix.UpdateOppCards();

        _lastCount = _matrix.OppCards.Length;
        _choiceIndex = new int[_lastCount, _lastCount];
        for (int r = 0; r < _lastCount; r++) {
            for (int c = 0; c < _lastCount; c++) {
                PlayingCard oppCard = _matrix.OppCards[c];
                PlayingCard playerCard = _matrix.OppCards[r];
                _choiceIndex[r, c] = (int)_matrix.GetMatch(oppCard, playerCard);
            }
        }

        RegenerateTableHeaders();
    }

    private void RegenerateTableHeaders()
    {
        _oppCardNames = _matrix.OppCards.Select(x => (x != null) ? x.Id : "Null").ToArray();
    }

    private string GetShortId(int i)
    {
        // return $"{i}";

        if (i < 0 || i >= _oppCardNames.Length) {
            Debug.LogError($"BarterResponseMatrix Editor Error: GetShortId failed. Index ({i}) was "
                         + $"out of range of _oppCardNames (length {_oppCardNames.Length}).");
            return "Null";
        }

        // Take our abbreviated length, or the full id length, whichever is shorter.
        string id = _oppCardNames[i];
        int length = (int)Mathf.Min(_idLength, id.Length);

        return id[0..length];
    }
}
#endif