using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "New BarterResponseMatrix", menuName = "Bartering/BarterResponseMatrix")]
public class BarterResponseMatrix : ScriptableObject
{
    public enum State {
        POSITIVE,
        NEUTRAL,
        NEGATIVE
    }
    
    // Parameters and Publics =====================================================================

    [Tooltip("An ID used for debug purposes.")]
    public string id;
    public State[] state = new State[16];

    // Manipulator Methods ========================================================================

    /// <summary>
    /// Populate our internal response lookup table. MUST be called before GetResponse() is used.
    /// </summary>
    public void Initialize()
    {
        return;
    }

    /// <summary>
    /// Takes a playing card, returns what this opponent wants as a response. 
    /// </summary>
    /// <param name="key">PlayingCard - the input card (used by the opponent).</param>
    /// <returns>PlayingCard - the output card that is a valid response to the input.</returns>
    public PlayingCard GetResponse(PlayingCard opp, PlayingCardMatch player)
    {
        return null;
    }

    // Helper structs =============================================================================

    /// <summary>
    /// A helper struct that ties a key PlayingCard to a value PlayingCard. The value is encoded
    /// as the 'correct' response to the key.
    /// 
    /// For now, only one correct response is supported.
    /// </summary>
    [System.Serializable]
    public struct PlayingCardMatch
    {
        [Tooltip("In a given matchup, the PlayingCard the opponent plays.")]
        public PlayingCard OppCard;
        [Tooltip("In a given matchup, the PlayingCard the player plays in response to the opponent.")]
        public PlayingCard PlayerCard;
        [Tooltip("The value of the this matchup.")]
        public State MatchState;
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BarterResponseMatrix))]
public class EditorSpawnTemplate : Editor
{
    private BarterResponseMatrix matrix;
    // Check, empty character, or cross
    private string[] _stateOpts = new []{"✓", "‎", "✗"};
    private string[] _cardTypes = new []{"Anger", "Bluff", "Direct", "Flirt"};
    private int[] _choiceIndex = new int[16];

    private void OnEnable()
    {
        matrix = (BarterResponseMatrix)target;

        for (int i=0; i < _choiceIndex.Length; i++) {
            _choiceIndex[i] = (int)matrix.state[i];
        }
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.BeginHorizontal();
        EditorGUIUtility.labelWidth = 15;
            EditorGUILayout.LabelField("Opponent >", EditorStyles.miniLabel);
            for (int x = 0; x < 4; x++) {
                EditorGUILayout.LabelField(_cardTypes[x], EditorStyles.boldLabel);
            }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.LabelField("Player v", EditorStyles.miniLabel);

        for (int y = 0; y < 4; y++) {
            EditorGUILayout.BeginHorizontal();

                EditorGUIUtility.labelWidth = 15;
                EditorGUILayout.LabelField(_cardTypes[y], EditorStyles.boldLabel);

                for (int x = 0; x < 4; x++) {
                    int i = x + y*4;

                    Color c = (BarterResponseMatrix.State)_choiceIndex[i] switch {
                        BarterResponseMatrix.State.POSITIVE => Color.green,
                        BarterResponseMatrix.State.NEGATIVE => Color.red,
                        _ => Color.white
                    };

                    GUI.backgroundColor = c;
                    _choiceIndex[i] = EditorGUILayout.Popup(_choiceIndex[i], _stateOpts);
                    matrix.state[i] = (BarterResponseMatrix.State)_choiceIndex[i];
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
}
#endif