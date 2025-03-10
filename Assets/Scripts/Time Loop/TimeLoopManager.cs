using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class TimeLoopManager : MonoBehaviour
{
    // Works as a modified, lightweight singleton.
    public static TimeLoopManager Instance { get; private set; }

    // Parameters and publics =====================================================================

    [SerializeField, Tooltip("The amount of time, in minutes, it takes for the loop to end.")] 
    private float loopMinutes = 8;
    // Public accessor, for other scripts
    public static float SecondsLeft => Instance._secondsLeft;
    // Read-only display, for the inspector
    [SerializeField, ReadOnly] 
    private string DEBUG_timeLeft;

    // Public accessor, for other scripts
    public static bool LoopPaused  => Instance._loopPaused;

    // Called when the loop time is fully elapsed. Awaits a callback.
    public static System.Action<System.Action> LoopElapsed;

    // Misc Internal Variables ====================================================================

    private float _secondsLeft;
    private bool _loopPaused;
    private bool _loopDone;

    // Intializers ================================================================================

    private void Awake()
    {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (isActiveAndEnabled) {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void Start()
    {
        if (isActiveAndEnabled) {    
            InitializeTimer();
        }
    }

    public void InitializeTimer()
    {
        _secondsLeft = loopMinutes * 60;

        _loopDone = false;
        _loopPaused = false;
    }

    // Finalizers =================================================================================

    public void CallbackDone() 
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        GameManager.Instance.FindPlayer();
        GameManager.Instance.FindMasterCanvas();
        InitializeTimer(); 

        // TODO: Port this code over into InGameUi, using a MoveToDefault() call
        GameManager.Player.Movement.SetCanMove(true);
        GameManager.Player.InteractionHandler.SetCanInteract(true);
        GameManager.PlayerInput.AllowNavbar = true;

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update methods =============================================================================

    void Update()
    {
        if (_loopDone || _loopPaused) return;

        DEBUG_timeLeft = $"{Mathf.Floor(_secondsLeft/60f):00}:{Mathf.Floor(_secondsLeft%60):00}";
        _secondsLeft -= Time.deltaTime;

        if (_secondsLeft <= 0) {
            _loopDone = true;

            // TODO: Port this code over into InGameUi, using a MoveTo???() call
            GameManager.Player.Movement.SetCanMove(false);
            GameManager.Player.InteractionHandler.SetCanInteract(false);
            GameManager.PlayerInput.AllowNavbar = false;

            // LoopElapsed is null with 0 subscribers, and non-null otherwise.
            if (LoopElapsed != null) {
                LoopElapsed.Invoke(CallbackDone);
            } else {
                CallbackDone();
            }            
        }
    }

    // Misc manipulators ==========================================================================

    public static void SetLoopPaused(bool value)
    {
        Instance._loopPaused = value;
    }
    
    public static void ResetLoop(){
        Instance._secondsLeft = 0;
    }
}
