using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    private ResolutionFullscreenHandler resFullscreen;
    //private AccessibilitySettingsHandler accessibility;

    public void Initialize()
    {
        // Each individual settings script is responsible for loading from / saving to prefs.
        // THIS FUNCTION IS CALLED ONLY ONCE PER SCENE, in Awake().
        // ================
        
        //volumeHandler = GetComponentInChildren<FMODVolumeHandler>(includeInactive:true);
        //volumeHandler.Initialize();

        resFullscreen = GetComponentInChildren<ResolutionFullscreenHandler>(includeInactive:true);
        resFullscreen.Initialize();

        //accessibility = GetComponentInChildren<AccessibilitySettingsHandler>(includeInactive:true);
        //accessibility.Initialize();
    }

    private void Start()
    {
        Initialize();
    }
}