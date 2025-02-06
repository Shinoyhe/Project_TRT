using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevMenu : Singleton<DevMenu> {
    [SerializeField] GameObject devMenu;
    float _initalTimescale = 0;
    bool _devOn = false;
    UiInputHandler _input => UiInputHandler.Instance;
    
    private void Start() {
        devMenu.SetActive(false);
    }
    
    // Update is called once per frame
    void Update() {
        if (_input == null) { return; }
        
        if (_input.GetDebugDown() == true) { ToggleDevMenu(); }
    }
    
    void ToggleDevMenu() {
        _devOn = !_devOn;
        devMenu.SetActive(_devOn);
        _initalTimescale = _devOn ? Time.timeScale : _initalTimescale;
        Time.timeScale = _devOn ? 0 : _initalTimescale;
    }
    
    void CloseDevMenu(Scene s, LoadSceneMode lsm){
        _devOn = false;
        devMenu.SetActive(_devOn);
        Time.timeScale = 1;
    }
    
    private void OnEnable() {
        SceneManager.sceneLoaded += CloseDevMenu;
    }
    
    private void OnDisable() {
        SceneManager.sceneLoaded -= CloseDevMenu;
    }
}
