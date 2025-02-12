using UnityEngine;
using NaughtyAttributes;

public class GameManager : Singleton<GameManager>
{
    // Public accessors ===========================================================================

    public static DialogueManager DialogueManager { get { return Instance.dialogueManager; } }
    public static PlayerInputHandler PlayerInput { get { return Instance.playerInput; } }
    public static UiInputHandler UiInput { get { return Instance.uiInput; } }
    public static Inventory Inventory { get { return Instance.inventory; } }
    public static TimeLoopManager TimeLoopManager { get { return Instance.timeLoopManager; } }
    public static Player Player { get { return Instance.player; } }

    // Backing fields =============================================================================

    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerInputHandler playerInput;
    [SerializeField] private UiInputHandler uiInput;
    [SerializeField] private Inventory inventory;
    [SerializeField] private TimeLoopManager timeLoopManager;
    [SerializeField, Tag] private string playerTag;
    [SerializeField] private Player player;

    // Initializers ===============================================================================

    public void FindPlayer()
    {
        GameObject playerParent = GameObject.FindWithTag(playerTag).transform.root.gameObject;

        if (playerParent != null) {
            player = playerParent.GetComponentInChildren<Player>();
        }
    }
}
