using UnityEngine;
using NaughtyAttributes;

public class GameManager : Singleton<GameManager>
{
    // Public accessors ===========================================================================

    public static DialogueManager DialogueManager { get { return Instance.dialogueManager; } }
    public static PlayerInputHandler PlayerInput { get { return Instance.playerInput; } }
    public static Inventory Inventory { get { return Instance.inventory; } }
    public static Player Player { get { return Instance._player; } }
    public static Canvas MasterCanvas { get { return Instance._masterCanvas; } }
    public static BarterStarter BarterStarter { get { return Instance.barterStarter; } }

    // Backing fields =============================================================================

    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private PlayerInputHandler playerInput;
    [SerializeField] private Inventory inventory;
    [SerializeField, Tag] private string playerTag;
    [SerializeField, ReadOnly] private Player _player;
    [SerializeField, Tag] private string masterCanvasTag;
    [SerializeField, ReadOnly] private Canvas _masterCanvas;
    [SerializeField] private BarterStarter barterStarter;

    // Initializers ===============================================================================

    public void FindPlayer()
    {
        print("FindPlayer() called");

        GameObject playerObj = GameObject.FindWithTag(playerTag);

        if (playerObj != null) {
            GameObject playerParent = playerObj.transform.root.gameObject;

            if (playerParent != null) {
                _player = playerParent.GetComponentInChildren<Player>();
            }
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
