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
    public float VerticalOffset = 0.25f;

    // Initializers and Update ================================================================

    private void Update() {

        // Check for Player Input
        if (PlayerInputHandler.Instance.GetJumpInputDown()) {
            OnInteract();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    // Private Helper Methods =====================================================================

    /// <summary>
    /// Called by interaction mananger to trigger dialogue.
    /// </summary>
    private void OnInteract() {
        var positionOfBubble = transform.position + BubbleSpawnOffset + new Vector3(0,VerticalOffset,0);
        
        DialogueManager.Instance.StartConversation(InkFile, positionOfBubble);
    }

    /// <summary>
    /// Display draw position.
    /// </summary>
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(this.transform.position + BubbleSpawnOffset, 0.25f);
    }
}
