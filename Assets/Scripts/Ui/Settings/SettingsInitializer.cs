using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsInitializer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ResolutionFullscreenHandler.InitFromPrefs();
        AccessibilitySettingsHandler.InitFromPrefs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
