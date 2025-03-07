using NaughtyAttributes;
using UnityEngine;

public class BarterStarter : MonoBehaviour
{
    // Parameters =================================================================================
    [SerializeField] private GameObject barterContainerPrefab;
    [SerializeField] private GameObject presentItemPrefab;

    [BoxGroup("Barter Settings"), ReadOnly] public NPCData NpcData;
    [BoxGroup("Barter Settings"), ReadOnly] public BarterResponseMatrix BarterResponseMatrix;
    [BoxGroup("Barter Settings"), ReadOnly] public BarterNeutralBehavior BarterNeutralBehaviour;
    [BoxGroup("Barter Settings"), ReadOnly] public Trades PossibleTrades;
    [BoxGroup("Barter Settings"), ReadOnly] public Trade CurrentTrade;
    [BoxGroup("Barter Settings"), ReadOnly] public float BaseDecay = 1;
    [BoxGroup("Barter Settings"), ReadOnly] public float DecayAcceleration = 0.025f;
    [BoxGroup("Barter Settings"), ReadOnly] public float WillingnessPerMatch = 5;
    [BoxGroup("Barter Settings"), ReadOnly] public float WillingnessPerFail = -5;
    [BoxGroup("Barter Settings"), ReadOnly] public float StartingWillingness = 50;

    // Win/Lose Actions
    public System.Action OnWin;
    public System.Action OnLose;

    // Misc Internal Variables ====================================================================

    private InGameUi _inGameUi;
    private GameObject _barterInstance;
    private GameObject _itemPresentInstance;

    // Initializers ===============================================================================

    private void Start()
    {
        _inGameUi = GameManager.MasterCanvas.GetComponentInChildren<InGameUi>();

        // TODO: Replace this with non-debug functionality.
        OnWin += () => Debug.Log("BarterStarter: OnWin called!");
        OnLose += () => Debug.Log("BarterStarter: OnLose called!");
    }  
    
    // Pre-Barter Methods =========================================================================

    public void PresentItem()
    {
        _itemPresentInstance = Instantiate(presentItemPrefab, GameManager.MasterCanvas.transform);
        RectTransform rectTransform = _itemPresentInstance.GetComponent<RectTransform>();

        PresentItem presentItem = _itemPresentInstance.GetComponent<PresentItem>();
        presentItem.PossibleTrades = PossibleTrades;
        presentItem.OnAccepted += AcceptTrade;
        presentItem.OnClosed += CloseTrade;

        GameManager.PlayerInput.IsActive = false;

        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(true);
        }
    }

    private void AcceptTrade()
    {
        CleanupPresentationCanvas(false);
        StartBarter();
    }

    private void CloseTrade()
    {
        CleanupPresentationCanvas(true);
    }

    private void CleanupPresentationCanvas(bool enablePlayerInput)
    {
        Destroy(_itemPresentInstance);
        GameManager.PlayerInput.IsActive = enablePlayerInput;

        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(false);
        }
    }

    // StartBarter() ==============================================================================

    /// <summary>
    /// Starts the Barter
    /// </summary>
    /// <returns>GameObject - an initialized and active BarterContainer.</returns>
    public GameObject StartBarter()
    {
        // Create the BarterContainer.
        _barterInstance = Instantiate(barterContainerPrefab, Vector3.zero, Quaternion.identity,
                                      GameManager.MasterCanvas.transform);
        // Set the InGameUi state to the BarteringState.
        _inGameUi.MoveToBartering();

        BarterDirector _barterDirector = _barterInstance.GetComponentInChildren<BarterDirector>();
        _barterDirector.OnWin += WinBarter;
        _barterDirector.OnLose += LoseBarter;

        // Set up the BarterDirector settings
        _barterDirector.InitializeWillingness(StartingWillingness);

        if (NpcData != null) { 
            _barterDirector.NpcData = NpcData;
        } else {
            Debug.LogError("BarterStarter: Could not find NpcData");
        }

        if (BarterResponseMatrix != null) { 
            _barterDirector.BarterResponses = BarterResponseMatrix;
        } else {
            Debug.LogError("BarterStarter: Could not find BarterResponseMatrix");
        }

        if (BarterNeutralBehaviour != null) {
            _barterDirector.NeutralBehavior = BarterNeutralBehaviour;
        } else {
            Debug.LogError("BarterStarter: Could not find BarterNeutralBehaviour");
        }

        _barterDirector.BaseDecay = BaseDecay;
        _barterDirector.DecayAcceleration = DecayAcceleration;
        _barterDirector.WillingnessPerMatch = WillingnessPerMatch;
        _barterDirector.WillingnessPerFail = WillingnessPerFail;

        GameManager.PlayerInput.IsActive = false;
        //MusicManager.play

        return _barterInstance;
    }

    // Post-Barter Methods ========================================================================

    /// <summary>
    /// Called via action invocation on Barter win, gives player the prize card.
    /// </summary>
    private void WinBarter()
    {
        if (CurrentTrade != null) {
            GameManager.Inventory.AddCard(CurrentTrade.RewardCard);

            if (CurrentTrade.AcceptedCard.Type == GameEnums.CardTypes.ITEM) {
                GameManager.Inventory.RemoveCard(CurrentTrade.AcceptedCard);
            }
        } else {
            Debug.LogError("Failed to reward card after win, CurrentTrade was not set");
        }

        OpenJournal(OnWin);
    }

    /// <summary>
    /// Called via action invocation on Barter lose.
    /// </summary>
    private void LoseBarter()
    {
        OpenJournal(OnLose);
    }

    /// <summary>
    /// Shared functionality for WinBarter and LoseBarter which opens the journal, and invokes a
    /// callback function when the UiStates returns to the Default state.
    /// </summary>
    /// <param name="closeCallback">System.Action - invoked when the Journal is closed.</param>
    private void OpenJournal(System.Action closeCallback)
    {
        _inGameUi.MoveToJournal();
        CleanupBarter();

        _inGameUi.CanvasStateChanged += (oldState, newState) => {
            if (newState == InGameUi.UiStates.Default){
                closeCallback?.Invoke();
            }
        };
    }

    /// <summary>
    /// Handles cleanup of barter minigame.
    /// </summary>
    private void CleanupBarter()
    {
        Destroy(_barterInstance);
        GameManager.PlayerInput.IsActive = true;
    }
}