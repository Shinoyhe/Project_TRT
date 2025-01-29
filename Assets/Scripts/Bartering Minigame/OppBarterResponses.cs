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

    private readonly Dictionary<string, PlayingCard> _responseDict = new();

    // Manipulator Methods ========================================================================

    public void Initialize()
    {
        // Clear the dict before populating it.
        _responseDict.Clear();

        foreach (PlayingCardPair pair in PlayingCardPairs) {
            // No double-entries.
            if (_responseDict.ContainsKey(pair.Key.Id)) {
                Debug.LogError("OppBarterResponses Error: Initialize failed. _responseDict "
                            + $"contains key with ID {pair.Key.Id}.");
                return;
            }

            _responseDict[pair.Key.Id] = pair.Value;
        }

        // The dict is now initialized and ready to use!
    }

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

    [System.Serializable]
    public struct PlayingCardPair
    {
        [Tooltip("In a given matchup, the PlayingCard the opponent plays.")]
        public PlayingCard Key;
        [Tooltip("In a given matchup, the PlayingCard the player plays in response to the opponent.")]
        public PlayingCard Value;
    }
}