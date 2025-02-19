using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class TimeLoopManager : MonoBehaviour
{
    // Parameters and publics =====================================================================

    [SerializeField, Tooltip("The amount of time, in minutes, it takes for the loop to end.")] 
    private float loopMinutes = 8;
    // Public accessor, for other scripts
    public float SecondsLeft => _secondsLeft;
    // Read-only display, for the inspector
    [SerializeField, ReadOnly] 
    private string DEBUG_timeLeft;

    // Public accessor, for other scripts
    public bool LoopPaused  => _loopPaused;

    // Called when the loop time is fully elapsed. Awaits a callback.
    public System.Action<System.Action> LoopElapsed;

    // Misc Internal Variables ====================================================================

    private float _secondsLeft;
    private bool _loopPaused;
    private bool _loopDone;

    // Intializers ================================================================================

    private void Awake()
    {
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
        GameManager.PlayerInput.IsActive = true;
        GameManager.UiInput.IsActive = true;

        GameManager.Instance.FindPlayer();
        GameManager.Instance.FindMasterCanvas();
        InitializeTimer();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update methods =============================================================================

    void Update()
    {
        // Adding a test comment.
        
        if (_loopDone || _loopPaused) return;

        DEBUG_timeLeft = $"{Mathf.Floor(_secondsLeft/60f):00}:{Mathf.Floor(_secondsLeft%60):00}";
        _secondsLeft -= Time.deltaTime;

        if (_secondsLeft <= 0) {
            _loopDone = true;

            GameManager.PlayerInput.IsActive = false;
            GameManager.UiInput.IsActive = false;

            // LoopElapsed is null with 0 subscribers, and non-null otherwise.
            if (LoopElapsed != null) {
                LoopElapsed.Invoke(CallbackDone);
            } else {
                CallbackDone();
            }            
        }
    }

    // Misc manipulators ==========================================================================

    public void SetLoopPaused(bool value)
    {
        _loopPaused = value;
    }
    
    public void ResetLoop(){
        _secondsLeft = 0;
    }
}
