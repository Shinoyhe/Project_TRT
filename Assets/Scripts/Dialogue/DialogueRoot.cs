using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueRoot : MonoBehaviour
{
    // Parameters =================================================================================

    [Header("Traits")]
    public Vector3 BubbleSpawnOffset;
    public TextAsset InkFile;

    // Initializers and Update ================================================================

    private void Update() {

        // Check for Player Input
        if (Input.GetKeyDown(KeyCode.J)) {
            OnInteract();
        }
    }

    // Private Helper Methods =====================================================================

    /// <summary>
    /// Called by interaction mananger to trigger dialogue.
    /// </summary>
    private void OnInteract() {
        var positionOfBubble = transform.position + BubbleSpawnOffset;
        
        DialogueManager.Instance.StartConversation(InkFile, positionOfBubble);
    }

    /// <summary>
    /// Display draw position.
    /// </summary>
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(this.transform.position + BubbleSpawnOffset, 0.25f);
    }
}
