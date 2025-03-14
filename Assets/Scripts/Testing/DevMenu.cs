using UnityEngine;
using UnityEngine.SceneManagement;

public class DevMenu : Singleton<DevMenu> {
    [SerializeField] GameObject devMenu;

    private float _initalTimescale = 0;
    private bool _devOn = false;
    
    private void Start()
    {
        devMenu.SetActive(false);
    }
    
    // Update is called once per frame
    void Update()
    {
        if (GameManager.PlayerInput == null) return;
        else if (GameManager.PlayerInput.GetDebug0Down()) ToggleDevMenu();
        else if (GameManager.PlayerInput.GetDebug1Down()) ResetGame();
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
    
    
    #region Public Methods
    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ResetLoop()
    {
        CloseDevMenu();
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(false);
            TimeLoopManager.ResetLoop();
        }
    }
    
    public void PauseLoop()
    {
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(!TimeLoopManager.LoopPaused);
        }
    }
    
    public void ResetGame(){
        if (GameManager.Inventory != null) {
            GameManager.Inventory.Clear();
        }
        SceneManager.LoadScene(0);
    }
    
    public void GiveAllItems()
    {
        if (GameManager.Inventory != null)
        {
            foreach (InventoryCardData data in GameManager.Inventory.AllCardDatas)
            {
                if (GameManager.Inventory.HasCard(data)) continue;
                GameManager.Inventory.AddCard(data);
            }
        }
    }
    #endregion
}
