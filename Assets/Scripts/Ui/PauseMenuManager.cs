using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenuManager : Singleton<PauseMenuManager>
{
    public GameObject UiParent;

    private bool _paused = false;

    private void Start() {
        UiParent.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerInputHandler.Instance.GetPauseDown()) { 

            if (_paused) {
                UnPauseScene();
                _paused = false;
            } else {
                PauseScene();
                _paused = true;
            }
        }
    }

    void PauseScene() {
        Time.timeScale = 0;

        UiParent.SetActive(true);

    }

    void UnPauseScene() {
        Time.timeScale = 1;
        UiParent.SetActive(false);
    }
}
