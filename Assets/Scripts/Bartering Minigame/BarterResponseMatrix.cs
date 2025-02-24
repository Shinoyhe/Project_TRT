using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New BarterResponseMatrix", menuName = "Bartering/BarterResponseMatrix")]
public class BarterResponseMatrix : ScriptableObject
{
    // Helper classes and enums ===================================================================

    public enum State {
        POSITIVE,
        NEUTRAL,
        NEGATIVE,
    }

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

    // Parameters and Publics =====================================================================

    [Tooltip("An ID used for debug purposes.")]
    public string id;
    [SerializeField, Tooltip("The cards that this opponent can play.")]
    private PlayingCard[] oppCards = new PlayingCard[0];
    public PlayingCard[] OppCards => oppCards;
    [Tooltip("The default state returned whenever the player plays a card not in OppCards.")]
    public State defaultState = State.NEUTRAL;

    // Misc Internal Variables ====================================================================

    [SerializeField] private List<CardCardStateTriplet> _editorMatchList;
    public Dictionary<PlayingCard, Dictionary<PlayingCard, State>> _runtimeMatchDict;    

    // Runtime methods ============================================================================

    /// <summary>
    /// Called once at runtime, before this matrix is ever read from. Loads our triplet list into
    /// our runtime dictionary.
    /// </summary>
    public void InitializeDict()
    {
        if (_runtimeMatchDict == null) {
            _runtimeMatchDict = new();
        } else {
            _runtimeMatchDict.Clear();
        }

        foreach (var triplet in _editorMatchList) {
            if (!_runtimeMatchDict.ContainsKey(triplet.OppCard)) {
                _runtimeMatchDict[triplet.OppCard] = new();
            }

            _runtimeMatchDict[triplet.OppCard][triplet.PlayerCard] = triplet.Match;
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

        if (!_runtimeMatchDict[oppCard].ContainsKey(playerCard)) {
            return defaultState;
        }

        // Else, our indexing is valid!
        return _runtimeMatchDict[oppCard][playerCard];
    }

    // Editor-time methods ========================================================================

    private void OnEnable()
    {
        UpdateTripletsEditor();
    }

    private void UpdateTripletsEditor()
    {
        // Updates our dictionary based on the contents of our OppCards list.
        // ================

        // Lazily initialize- if null, create.
        _editorMatchList ??= new();

        // Remove missing cards.
        // Iterate through an array copy, so we can remove from the list without getting changed-
        // while-iterating errors.
        foreach (var triplet in _editorMatchList.ToArray()) {
            // If one of the triplet's cards isn't in our valid list, remove the triplet.
            if (!OppCards.Contains(triplet.OppCard) || !OppCards.Contains(triplet.PlayerCard)) {
                _editorMatchList.Remove(triplet);
            }
        }

        // Add new cards.
        foreach (PlayingCard oppCard in OppCards) {
            foreach (PlayingCard playerCard in OppCards) {
                // If the pair doesn't exist, add it.
                if (!TripletExistsEditor(oppCard, playerCard)) {
                    _editorMatchList.Add(new(oppCard, playerCard, State.NEUTRAL));
                }
            }
        }
    }

    private void SetMatchEditor(PlayingCard oppCard, PlayingCard playerCard, State match)
    {
        // Searches _editorMatchList for a CardCardState triplet that matches both cards, then 
        // set that triplet's state to match.
        // Searching the list manually is expensive, so we only use this at editor-time.
        // ================

        var triplet = _editorMatchList.Find(x => x.OppCard == oppCard && 
                                            x.PlayerCard == playerCard);
        triplet.Match = match;
    }

    private State GetMatchEditor(PlayingCard oppCard, PlayingCard playerCard)
    {
        // Searches _editorMatchList for a CardCardState triplet that matches both cards, then 
        // returns that triplet's state.
        // Searching the list manually is expensive, so we only use this at editor-time.
        // ================

        var triplet = _editorMatchList.Find(x => x.OppCard == oppCard && 
                                            x.PlayerCard == playerCard);
        return triplet.Match;
    }

    private bool TripletExistsEditor(PlayingCard oppCard, PlayingCard playerCard)
    {
        // Searches _editorMatchList for a CardCardState triplet that matches both cards, then 
        // returns that triplet's state.
        // Searching the list manually is expensive, so we only use this at editor-time.
        // ================

        var triplet = _editorMatchList.Find(x => x.OppCard == oppCard && 
                                            x.PlayerCard == playerCard);
        return triplet != null;
    }

    private void CleanOppCardsEditor()
    {
        // Remove duplicates and null cards from oppCards.
        // ================

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
        // As of the last time we checked, the length of _matrix.oppCards
        private int _lastCount = 0;
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

            // Multispace(3);
            // EditorGUILayout.PropertyField(serializedObject.FindProperty("_editorMatchList"));

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

            _matrix.CleanOppCardsEditor();
            _matrix.UpdateTripletsEditor();

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
            // Recreate the _cardIds array, used for matrix headers.
            // ================

            _cardIds = _matrix.oppCards.Select(x => (x != null) ? x.Id : "Null").ToArray();
        }

        //  Visual helpers ========================================================================

        private string GetShortId(int i)
        {
            // Given a index in our _cardIds list, returns the header at that index, abbreviated to
            // _idLength.
            // ================

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
            // Creates a horizontal line across the editor GUI.
            // ================
            
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