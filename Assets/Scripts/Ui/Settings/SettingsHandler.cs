using UnityEngine;

public class SettingsHandler : MonoBehaviour
{
    private ResolutionFullscreenHandler resFullscreen;
    private AccessibilitySettingsHandler accessibility;

    public void Initialize()
    {
        // Each individual settings script is responsible for loading from / saving to prefs.
        // THIS FUNCTION IS CALLED ONLY ONCE PER SCENE, in Awake().
        // ================
        
        //volumeHandler = GetComponentInChildren<FMODVolumeHandler>(includeInactive:true);
        //volumeHandler.Initialize();

        resFullscreen = GetComponent<ResolutionFullscreenHandler>();
        resFullscreen.Initialize();

        accessibility = GetComponent<AccessibilitySettingsHandler>();
        accessibility.Initialize();
    }

    private void Start()
    {
        Initialize();
    }
}