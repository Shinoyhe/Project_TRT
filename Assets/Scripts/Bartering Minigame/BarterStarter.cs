using Ink.Runtime;
using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class BarterStarter : MonoBehaviour
{
    // Parameters =================================================================================
    [SerializeField] private GameObject barterContainerPrefab;
    [SerializeField] private GameObject presentItemPrefab;
    [SerializeField] private GameObject winScreenPrefab;

    [BoxGroup("Barter Settings"), ReadOnly] public NPCData NpcData;
    [BoxGroup("Barter Settings"), ReadOnly] public BarterResponseMatrix BarterResponseMatrix;
    [BoxGroup("Barter Settings"), ReadOnly] public BarterNeutralBehavior BarterNeutralBehaviour;
    [BoxGroup("Barter Settings"), ReadOnly] public Trades PossibleTrades;
    [BoxGroup("Barter Settings"), ReadOnly] public Trade CurrentTrade;
    [BoxGroup("Barter Settings"), ReadOnly] public bool JournalOnWin = true;
    [BoxGroup("Barter Settings"), ReadOnly] public bool JournalOnLose = true;
    [BoxGroup("Barter Settings"), ReadOnly] public float BaseDecay = 1;
    [BoxGroup("Barter Settings"), ReadOnly] public float DecayAcceleration = 0.025f;
    [BoxGroup("Barter Settings"), ReadOnly] public float WillingnessPerMatch = 5;
    [BoxGroup("Barter Settings"), ReadOnly] public float WillingnessPerFail = -5;
    [BoxGroup("Barter Settings"), ReadOnly] public float StartingWillingness = 50;
    [BoxGroup("Barter Settings")] public float WinScreenDurationSeconds = 2;

    // Win/Lose Actions
    public System.Action OnWin;
    public System.Action OnLose;

    // SFX For winning and losing a barter (i gotta find a more appropriate place for this)
    [SerializeField] AudioEvent barterWinSFX;
    [SerializeField] AudioEvent barterLoseSFX;

    // Misc Internal Variables ====================================================================

    private InGameUi _inGameUi;
    private GameObject _barterInstance;
    private GameObject _itemPresentInstance;

    // Initializers ===============================================================================

    private void Start()
    {
        if (GameManager.MasterCanvas != null) {
            _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
        }

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

    private void CleanupPresentationCanvas(bool backToDefault)
    {
        Destroy(_itemPresentInstance);
        
        if (backToDefault) {
            _inGameUi.MoveToDefault();
        }

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

        if (GameManager.MasterCanvas != null) {
            _inGameUi = GameManager.MasterCanvas.GetComponent<InGameUi>();
        }

        // Set the InGameUi state to the BarteringState.
        _inGameUi.MoveToBartering();

        BarterDirector _barterDirector = _barterInstance.GetComponentInChildren<BarterDirector>();
        _barterDirector.OnWin += WinBarter;
        _barterDirector.OnLose += LoseBarter;

        // Set up the BarterDirector settings
        _barterDirector.InitializeWillingness(StartingWillingness);

        if (NpcData != null) { 
            _barterDirector.NpcData = NpcData;
            _barterDirector.barterNpcDisplay.UpdateData(NpcData);
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

        //MusicManager.play

        if (NpcData.Name == "B4rn3y") MusicActionsManager.ChangeMusicState("Barter_Mail");
        else MusicActionsManager.ChangeMusicState("Barter");

        return _barterInstance;
    }

    // Post-Barter Methods ========================================================================

    /// <summary>
    /// Called via action invocation on Barter win, gives player the prize card.
    /// </summary>
    private void WinBarter()
    {
        barterWinSFX.Play(gameObject);
        EndBarter(JournalOnWin, OnWin, true);
    }

    /// <summary>
    /// Called via action invocation on Barter lose.
    /// </summary>
    private void LoseBarter()
    {
        barterLoseSFX.Play(gameObject);
        EndBarter(JournalOnLose, OnLose, false);
    }

    /// <summary>
    /// Handles cleanup of barter minigame.
    /// </summary>
    /// <param name="openJournal">bool - whether to open the journal.</param>
    /// <param name="callback">System.Action - invoked to announce a win/loss.</param>
    private void EndBarter(bool openJournal, System.Action callback, bool won)
    {
        Destroy(_barterInstance);

        MusicActionsManager.ChangeToPreviousMusicState();


        EndSequenceParams parameters = new EndSequenceParams();
        parameters.openJournal = openJournal;
        parameters.callback = callback;
        parameters.won = won;

        StartCoroutine(BarterEndSequence(parameters));
    }

    /// <summary>
    /// Shared functionality for WinBarter and LoseBarter which opens the journal, and invokes a
    /// callback function when the UiStates returns to the Default state.
    /// </summary>
    /// <param name="closeCallback">System.Action - invoked when the Journal is closed.</param>
    private void OpenJournal(System.Action closeCallback, bool won)
    {
        _inGameUi.SetLastNonMenuState(InGameUi.UiStates.Default);
        _inGameUi.MoveToJournal(NpcData);

        _inGameUi.CanvasStateChanged += OnJournalClose;

        void OnJournalClose(InGameUi.UiStates oldState, InGameUi.UiStates newState)
        {
            // A named delegate function for CanvasStateChanged. 
            // Named and not anonymous so that we can unsubscribe ourselves.
            // ================
            
            if (newState == InGameUi.UiStates.Default || newState == InGameUi.UiStates.Dialogue)
            {
                ExchangeCards(won);
                _inGameUi.CanvasStateChanged -= OnJournalClose;
                closeCallback?.Invoke();
            }
        }
    }

    /// <summary>
    /// Gives the player the prize card promised, and removes the card they gave away.
    /// </summary>
    private void ExchangeCards(bool won)
    {
        if (!won) {
            return;
        }
        
        if (CurrentTrade != null) {
            GameManager.Inventory.AddCard(CurrentTrade.RewardCard);

            if (CurrentTrade.AcceptedCard.Type == GameEnums.CardTypes.ITEM) {
                GameManager.Inventory.RemoveCard(CurrentTrade.AcceptedCard);
            }

            GameManager.NPCGlobalList.GetNPCFromData(NpcData).AddJournalKnownTrade(CurrentTrade.AcceptedCard, CurrentTrade.RewardCard);
        } else {
            Debug.LogError("Failed to reward card after win, CurrentTrade was not set");
        }
    }

    /// <summary>
    /// Displays the winScreen, and then does the EndBarter operations
    /// </summary>
    private IEnumerator BarterEndSequence(EndSequenceParams parameters) 
    {
        bool openJournal = parameters.openJournal;
        System.Action callback = parameters.callback;
        bool won = parameters.won;

        GameObject winScreenObj = Instantiate(winScreenPrefab, Vector3.zero, Quaternion.identity,
                                      GameManager.MasterCanvas.transform);
        winScreenObj.GetComponent<BarterWinScreen>().Initialize(won, CurrentTrade.AcceptedCard, CurrentTrade.RewardCard, NpcData);

        yield return new WaitForSeconds(WinScreenDurationSeconds);

        if (openJournal)
        {
            OpenJournal(callback, won);
        }
        else
        {
            ExchangeCards(won);

            // TODO: Take us back to the conversation. In the meantime...
            _inGameUi.MoveToDefault();
            callback?.Invoke();
        }

        Destroy(winScreenObj);
    }

    private struct EndSequenceParams
    {
        public bool openJournal;
        public System.Action callback;
        public bool won;
    }
}