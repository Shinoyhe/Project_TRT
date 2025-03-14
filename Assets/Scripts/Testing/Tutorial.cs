using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(true);
        }
        Time.timeScale = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.PlayerInput == null) return;
        else if (GameManager.PlayerInput.GetAffirmDown()) CloseTutorial();
    }
    
    void CloseTutorial()
    {
        if (TimeLoopManager.Instance != null) {
            TimeLoopManager.SetLoopPaused(false);
        }
        
        Time.timeScale = 1;
        Destroy(gameObject);
    }
}