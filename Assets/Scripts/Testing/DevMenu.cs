using UnityEngine;
using UnityEngine.SceneManagement;

public class DevMenu : Singleton<DevMenu> {
    [SerializeField] GameObject devMenu;

    private float _initalTimescale = 0;
    private bool _devOn = false;
    private UiInputHandler _input => GameManager.UiInput;
    
    private void Start()
    {
        devMenu.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_input == null) { return; }
        if (_input.GetDebugDown() == true) { ToggleDevMenu(); }
    }
    
    void ToggleDevMenu() 
    {
        _devOn = !_devOn;

        devMenu.SetActive(_devOn);
        _initalTimescale = _devOn ? Time.timeScale : _initalTimescale;
        Time.timeScale = _devOn ? 0 : _initalTimescale;
    }
    
    void CloseDevMenu()
    {
        _devOn = false;
        
        devMenu.SetActive(_devOn);
        Time.timeScale = 1;
    }
    
    void CloseDevMenu(Scene s, LoadSceneMode lsm)
    {
        CloseDevMenu();
    }
    
    private void OnEnable()
    {
        SceneManager.sceneLoaded += CloseDevMenu;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= CloseDevMenu;
    }
    
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ResetLoop(){
        CloseDevMenu();
        GameManager.TimeLoopManager.ResetLoop();
    }
}
