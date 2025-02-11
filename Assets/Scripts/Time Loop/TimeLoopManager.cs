using UnityEngine;
using NaughtyAttributes;
using UnityEngine.SceneManagement;

public class TimeLoopManager : MonoBehaviour
{
    // Parameters and publics =====================================================================

    [SerializeField, Tooltip("The amount of time, in minutes, it takes for the loop to end.")] 
    private float loopMinutes = 8;
    // Public accessor, for other scripts
    public float SecondsLeft => _secondsLeft * 60;
    // Read-only display, for the inspector
    [SerializeField, ReadOnly] 
    private string DEBUG_timeLeft;
    // Called when the loop time is fully elapsed. Awaits a callback.
    public System.Action<System.Action> LoopElapsed;

    // Misc Internal Variables ====================================================================

    private float _secondsLeft;
    private bool _shouldCountDown;
    private bool _loopDone;

    // Intializers ================================================================================

    private void Start()
    {
        InitializeTimer();
    }

    public void InitializeTimer()
    {
        _secondsLeft = loopMinutes * 60;

        _loopDone = false;
        _shouldCountDown = true;
    }

    // Finalizers =================================================================================

    public void CallbackDone() 
    {
        // TODO: Take us to the stasis scene.

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void OnSceneLoaded(Scene _, LoadSceneMode __)
    {
        GameManager.PlayerInput.IsActive = true;
        GameManager.UiInput.IsActive = true;

        GameManager.Instance.FindPlayer();
        InitializeTimer();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update methods =============================================================================

    void Update()
    {
        if (_loopDone || !_shouldCountDown) return;

        DEBUG_timeLeft = $"{Mathf.Floor(_secondsLeft/60f):00}:{Mathf.Floor(_secondsLeft%60):00}";
        _secondsLeft -= Time.deltaTime;

        if (_secondsLeft <= 0) {
            _loopDone = true;

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
        _shouldCountDown = value;
    }
}
