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
    public static Player Player { get { return Instance._player; } }
    public static Canvas MasterCanvas { get { return Instance._masterCanvas; } }

    // Backing fields =============================================================================

    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerInputHandler playerInput;
    [SerializeField] private UiInputHandler uiInput;
    [SerializeField] private Inventory inventory;
    [SerializeField] private TimeLoopManager timeLoopManager;
    [SerializeField, Tag] private string playerTag;
    [SerializeField, ReadOnly] private Player _player;
    [SerializeField, Tag] private string masterCanvasTag;
    [SerializeField, ReadOnly] private Canvas _masterCanvas;

    // Initializers ===============================================================================

    public void FindPlayer()
    {
        print("FindPlayer() called");

        GameObject playerParent = GameObject.FindWithTag(playerTag).transform.root.gameObject;

        if (playerParent != null) {
            _player = playerParent.GetComponentInChildren<Player>();
        }
    }

    public void FindMasterCanvas()
    {
        print("FindMasterCanvas() called");

        GameObject masterCanvasObj = GameObject.FindWithTag(masterCanvasTag);

        if (masterCanvasObj != null) {
            _masterCanvas = masterCanvasObj.GetComponentInChildren<Canvas>();
        }
    }
}
