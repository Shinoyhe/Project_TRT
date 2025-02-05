using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "New OppBarterResponses", menuName = "Bartering/OppBarterResponses")]
public class OppBarterResponses : ScriptableObject
{
    // Parameters and Publics =====================================================================

    [Tooltip("An ID used for debug purposes.")]
    public string Id;
    [SerializeField, Tooltip("A list of key-value pairs, representing tones the opponent plays VS "
           + "what they want to hear in response.")]
    private PlayingCardPair[] PlayingCardPairs;

    // Misc Internal Variables ====================================================================

    // The dictionary populated from our list of PlayingCardPairs.
    // Serves as a lookup table for matching input Ids against response PlayingCards.
    private readonly Dictionary<string, PlayingCard> _responseDict = new();

    // Manipulator Methods ========================================================================

    /// <summary>
    /// Populate our internal response lookup table. MUST be called before GetResponse() is used.
    /// </summary>
    public void Initialize()
    {
        // Clear the dict before populating it.
        _responseDict.Clear();

        foreach (PlayingCardPair pair in PlayingCardPairs) {
            // No double-entries for keys.
            if (_responseDict.ContainsKey(pair.Key.Id)) {
                Debug.LogError("OppBarterResponses Error: Initialize failed. _responseDict "
                            + $"already contains key with ID {pair.Key.Id}.");
                return;
            }

            _responseDict[pair.Key.Id] = pair.Value;
        }

        // The dict is now initialized and ready to use!
    }

    /// <summary>
    /// Takes a playing card, returns what this opponent wants as a response. 
    /// </summary>
    /// <param name="key">PlayingCard - the input card (used by the opponent).</param>
    /// <returns>PlayingCard - the output card that is a valid response to the input.</returns>
    public PlayingCard GetResponse(PlayingCard key)
    {
        if (!_responseDict.ContainsKey(key.Id)) {
            Debug.LogError("OppBarterResponses Error: GetResponse failed. _responseDict does not "
                        + $"contain the requested key (ID {key.Id}).");
            return null;
        }

        return _responseDict[key.Id];
    }

    // Helper structs =============================================================================

    /// <summary>
    /// A helper struct that ties a key PlayingCard to a value PlayingCard. The value is encoded
    /// as the 'correct' response to the key.
    /// 
    /// For now, only one correct response is supported.
    /// </summary>
    [System.Serializable]
    public struct PlayingCardPair
    {
        [Tooltip("In a given matchup, the PlayingCard the opponent plays.")]
        public PlayingCard Key;
        [Tooltip("In a given matchup, the PlayingCard the player plays in response to the opponent.")]
        public PlayingCard Value;
    }
}