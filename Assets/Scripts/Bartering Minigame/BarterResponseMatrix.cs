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
        NEGATIVE,
    }

    [Tooltip("An ID used for debug purposes.")]
    public string id;
    [SerializeField, Tooltip("The cards that this opponent can play.")]
    private PlayingCard[] oppCards = new PlayingCard[0];
    public PlayingCard[] OppCards => oppCards;
    [Tooltip("The default state returned whenever the player plays a card not in OppCards.")]
    public State defaultState = State.NEUTRAL;

    [System.Serializable]
    public class CardCardStateTriplet {
        public PlayingCard OppCard;
        public PlayingCard PlayerCard;
        public State Match;

        public CardCardStateTriplet(PlayingCard oppCard, PlayingCard playerCard, State match)
        {
            OppCard = oppCard;
            PlayerCard = playerCard;
            Match = match;
        }
    }

    [SerializeField] private List<CardCardStateTriplet> editorMatchList;
    public Dictionary<PlayingCard, Dictionary<PlayingCard, State>> runtimeMatchDict;

    // Editor-time methods ========================================================================

    private void OnEnable()
    {
        UpdateTriplets();
    }

    // Runtime methods ============================================================================

    /// <summary>
    /// Called once at runtime, before this matrix is ever read from. Loads our triplet list into
    /// our runtime dictionary.
    /// </summary>
    public void InitializeDict()
    {
        if (runtimeMatchDict == null) {
            runtimeMatchDict = new();
        } else {
            runtimeMatchDict.Clear();
        }

        foreach (var triplet in editorMatchList) {
            if (!runtimeMatchDict.ContainsKey(triplet.OppCard)) {
                runtimeMatchDict[triplet.OppCard] = new();
            }

            runtimeMatchDict[triplet.OppCard][triplet.PlayerCard] = triplet.Match;
        }
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
            return State.NEUTRAL;
        }

        if (!runtimeMatchDict[oppCard].ContainsKey(playerCard)) {
            return defaultState;
        }

        // Else, our indexing is valid!
        return runtimeMatchDict[oppCard][playerCard];
    }

    // Editor-friend methods ======================================================================

    private void UpdateTriplets()
    {
        // Updates our dictionary based on the contents of our OppCards list.
        // ================

        // Lazily initialize- if null, create.
        // editorMatchList ??= new();

        // Remove missing cards.
        // Iterate through an array copy, so we can remove from the list without getting changed-
        // while-iterating errors.
        foreach (var triplet in editorMatchList.ToArray()) {
            // If one of the triplet's cards isn't in our valid list, remove the triplet.
            if (!OppCards.Contains(triplet.OppCard) || !OppCards.Contains(triplet.PlayerCard)) {
                editorMatchList.Remove(triplet);
            }
        }

        // Add new cards.
        foreach (PlayingCard oppCard in OppCards) {
            // If we don't have any triplets with our oppCard, instantiate them.
            if (!editorMatchList.Any(x => x.OppCard == oppCard || x.PlayerCard == oppCard)) {
                foreach (PlayingCard playerCard in OppCards) {
                    // Add the opponent-player pair...
                    editorMatchList.Add(new(oppCard, playerCard, State.NEUTRAL));
                    // and the player-opponent pair (if they're not the same)
                    if (oppCard != playerCard) {
                        editorMatchList.Add(new(playerCard, oppCard, State.NEUTRAL));
                    }
                }
            }
        }
        
    }

    private void SetMatchEditor(PlayingCard oppCard, PlayingCard playerCard, State match)
    {
        var triplet = editorMatchList.Find(x => x.OppCard == oppCard && x.PlayerCard == playerCard);
        triplet.Match = match;
    }

    private State GetMatchEditor(PlayingCard oppCard, PlayingCard playerCard)
    {
        var triplet = editorMatchList.Find(x => x.OppCard == oppCard && 
                                           x.PlayerCard == playerCard);
        return triplet.Match;
    }

    private void CleanUpOppCards()
    {
        // Remove duplicates and null cards.
        oppCards = oppCards.Distinct().Where(x => x != null).ToArray();
    }

    #region Editor class ==========================================================================

    #if UNITY_EDITOR 
    /// <summary>
    /// Script enclosing a custom editor for instances of the BarterResponseMatrix class.
    /// 
    /// The primary addition is a grid of enums that allows the bidimensional Card-Card-State
    /// dictionary to be set in the inspector!
    /// </summary>
    [CustomEditor(typeof(BarterResponseMatrix))]
    public class EditorSpawnTemplate : Editor
    {
        // The object that this editor applies to.
        private BarterResponseMatrix _matrix;

        // The three options for the enum checkbox- check, empty character, or cross.
        private static readonly string[] _stateOpts = new []{"✓", "‎", "✗"};
        // How many characters we trim IDs to, used for column/row headers.
        private static readonly uint _idLength = 3;
        // A GUIStyle used to make horizontal lines.
        private static readonly GUIStyle _horizontalLine = new();

        // An int array that holds the state of all the dropdowns.
        private int[,] _choiceIndex;
        // As of the last time we checked, the length of _matrix.OppCards
        private int _lastCount;
        // Where we store the IDs of cards, used to print headers.
        private string[] _cardIds;

        // Initializers ===========================================================================

        private void OnEnable()
        {
            _matrix = (BarterResponseMatrix)target;
            OppCardsChanged();
        }

        private void OnDisable()
        {
            OppCardsChanged();
        }

        // Primary Methods ========================================================================

        /// <summary>
        /// Overrides the default Inspector editor behavior.
        /// 
        /// The primary addition is a grid of enums that allows the bidimensional Card-Card-State
        /// dictionary to be set in the inspector!
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            Multispace(3);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("id"));
            Multispace(3);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("oppCards"));

            if (_matrix) {

                if (_matrix.OppCards.Length < _lastCount) OppCardsChanged();
                if (GUILayout.Button("Update OppCards in Matrix")) OppCardsChanged();

                Multispace(3);
                HorizontalLine(Color.grey);
                Multispace(3);

                // Write column labels
                EditorGUILayout.BeginHorizontal();
                    EditorGUIUtility.labelWidth = 1;
                    EditorGUILayout.LabelField("Opponent >", EditorStyles.miniLabel);
                    for (int c = 0; c < _lastCount; c++) {
                        EditorGUILayout.LabelField(GetShortId(c), EditorStyles.boldLabel);
                    }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.LabelField("Player v", EditorStyles.miniLabel);

                // Write rows
                for (int r = 0; r < _lastCount; r++) {
                    EditorGUILayout.BeginHorizontal();
                        // Write row labels
                        EditorGUIUtility.labelWidth = 1;
                        EditorGUILayout.LabelField(GetShortId(r), EditorStyles.boldLabel);
                        // Write columns of enum checkboxes
                        for (int c = 0; c < _lastCount; c++) {

                            GUI.backgroundColor = (State)_choiceIndex[r,c] switch {
                                State.POSITIVE => Color.green,
                                State.NEGATIVE => Color.red,
                                _ => Color.white
                            };

                            int newIndex = EditorGUILayout.Popup(_choiceIndex[r,c], _stateOpts);
                            // If changed, write back an update.
                            if (newIndex != _choiceIndex[r,c]) {
                                _choiceIndex[r,c] = newIndex;
                                _matrix.SetMatchEditor(_matrix.OppCards[c], _matrix.OppCards[r], 
                                                (State)newIndex);
                                
                                // If we changed one of the matchups, make sure we save to disk.
                                EditorUtility.SetDirty(_matrix);
                            }
                        }

                    EditorGUILayout.EndHorizontal();
                    // Reset the color after setting it for the enum checkbox.
                    GUI.backgroundColor = Color.white;
                }

                // Reset label width and GUI color.
                EditorGUIUtility.labelWidth = 0;
                GUI.backgroundColor = Color.white;
            }

            Multispace(3);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("defaultState"));

            // Save our changes to the asset.
            serializedObject.ApplyModifiedProperties();
        }

        private void OppCardsChanged()
        {
            // Called when OppCards changes (either automatically when it shrinks, or manually
            // when growing or permutating).
            // 
            // Validates the OppCards list, updates our triplet list to just have valid entries,
            // repaints our _choiceIndex array, and rewrites our headers.
            // ================

            _matrix.CleanUpOppCards();
            _matrix.UpdateTriplets();

            _lastCount = _matrix.OppCards.Length;
            _choiceIndex = new int[_lastCount, _lastCount];
            for (int r = 0; r < _lastCount; r++) {
                for (int c = 0; c < _lastCount; c++) {
                    PlayingCard oppCard = _matrix.OppCards[c];
                    PlayingCard playerCard = _matrix.OppCards[r];

                    _choiceIndex[r,c] = (int)_matrix.GetMatchEditor(oppCard, playerCard);
                }
            }

            RegenerateCardIds();
        }

        private void RegenerateCardIds()
        {
            _cardIds = _matrix.OppCards.Select(x => (x != null) ? x.Id : "Null").ToArray();
        }

        //  Visual helpers ========================================================================

        private string GetShortId(int i)
        {
            if (i < 0 || i >= _cardIds.Length) {
                Debug.LogError($"BarterResponseMatrix Editor Error: GetShortId failed. Index ({i}) was "
                            + $"out of range of _oppCardNames (length {_cardIds.Length}).");
                return "Null";
            }

            // Take our abbreviated length, or the full id length, whichever is shorter.
            string id = _cardIds[i];
            int length = (int)Mathf.Min(_idLength, id.Length);

            return id[0..length];
        }

        private static void HorizontalLine(Color color) 
        {
            _horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
            _horizontalLine.margin = new RectOffset(0, 0, 4, 4);
            _horizontalLine.fixedHeight = 1;

            var oldColor = GUI.color;
            GUI.color = color;
            GUILayout.Box(GUIContent.none, _horizontalLine);
            GUI.color = oldColor;
        }

        private static void Multispace(int times)
        {
            for (int i = 0; i < times; i++) EditorGUILayout.Space();
        }
    }
    #endif

    #endregion
}