using UnityEngine;

public class DialogueRoot : MonoBehaviour
{
    // Parameters =================================================================================

    [Header("Traits")]
    public Vector3 BubbleSpawnOffset;
    public TextAsset InkFile;
    public Sprite PlayerAsset;
    public float VerticalOffset = 0.25f;

    // Initializers and Update ================================================================

    private void Update() {

        // Check for Player Input
        if (GameManager.PlayerInput.GetDebug0Down()) {
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
        
        GameManager.DialogueManager.StartConversation(InkFile, "Test", PlayerAsset);
    }

    /// <summary>
    /// Display draw position.
    /// </summary>
    private void OnDrawGizmos() {
        Gizmos.DrawWireSphere(transform.position + BubbleSpawnOffset, 0.25f);
    }
}
