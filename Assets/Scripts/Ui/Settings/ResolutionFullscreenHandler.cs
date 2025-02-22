using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Works in editor!

// ================================================================================================================================
// Resolution and fullscreen code from Lance Talbert, on red-gate.com.
// Accessed from: https://www.red-gate.com/simple-talk/development/dotnet-development/how-to-create-a-settings-menu-in-unity/
// ================================================================================================================================
public class ResolutionFullscreenHandler : MonoBehaviour
{
    // ==============================================================
    // Parameters
    // ==============================================================

    [SerializeField, Tooltip("The dropdown which controls our resolution settings.")]
    TMP_Dropdown resolutionDropdown;
    [SerializeField, Tooltip("The toggle controlling our fullscreen status.")]
    Toggle fullscreenToggle;

    // ==============================================================
    // Misc. internal variables
    // ==============================================================

    private Vector2Int[] resolutions;
    private int resolutionIndex = 0;
    private int isFullscreen;

    // ==============================================================
    // Initializers/finalizers
    // ==============================================================

    public void Initialize()
    {
        // Initializes our resolution options array.
        // THIS FUNCTION IS CALLED ONLY ONCE PER SCENE, on Awake().
        // ================

        // ================
        // Fullscreen stuff
        // ================

        if (PlayerPrefs.HasKey("IsFullscreenPref"))
        {
            isFullscreen = PlayerPrefs.GetInt("IsFullscreenPref");
        }
        else
        { // Default value.
            isFullscreen = 1; // True
        }

        Screen.fullScreen = isFullscreen == 1; // If 1, then true

        // ================
        // Resolution stuff
        // ================

        // Set up the array.

        HashSet<Vector2Int> resolutionSet = new();
        foreach (Resolution r in Screen.resolutions)
        {
            // We want to add two things to this set, per default resolution.
            // 1. A square aspect ratio, with side length equal to the shortest side
            //    of the default resolution.
            // 2. The default resolution.
            // If the default resolution is shorter than 16:9, skip BOTH these steps!
            // ================

            // If the aspect ratio is equal to 16:9 or is taller...
            if (r.width / r.height <= 16f / 9f)
            {
                // Find the shortest dimension, and add a square version of it.
                int shortest = (r.width < r.height) ? r.width : r.height;
                resolutionSet.Add(new(shortest, shortest));

                resolutionSet.Add(new(r.width, r.height));
            }
        }
        resolutions = resolutionSet.ToArray();

        if (resolutionDropdown != null)
        {
            resolutionDropdown.ClearOptions();

            List<string> options = new();

            for (int i = 0; i < resolutions.Length; i++)
            {
                Vector2Int res = resolutions[i];
                string option = $"{res.x} x {res.y}";
                // The recommended resolution is the highest one.
                if (i == resolutions.Length - 1) option += " (Recommended)";
                options.Add(option);
            }

            resolutionDropdown.AddOptions(options);
        }

        // Load the saved preference.

        if (PlayerPrefs.HasKey("ResolutionIndexPref"))
        {
            resolutionIndex = PlayerPrefs.GetInt("ResolutionIndexPref");

            // If the cached index is past the bounds of our resolutions array, ditch it.
            if (resolutionIndex > resolutions.Length)
            {
                // Choose the best current resolution, instead.
                resolutionIndex = resolutions.Length;
            }
        }
        else
        { // Default value.
            resolutionIndex = resolutions.Length;
        }

        SetResolution(resolutionIndex);
    }

    private void OnEnable()
    {
        InitializeDisplay();
    }

    private void InitializeDisplay()
    {

        if (resolutionDropdown != null)
        {
            resolutionDropdown.value = resolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = isFullscreen == 1;
        }
    }

    private void OnDisable()
    {
        SaveToPrefs();
    }

    public void SaveToPrefs()
    {
        PlayerPrefs.SetInt("ResolutionIndexPref", resolutionIndex);
        PlayerPrefs.SetInt("IsFullscreenPref", isFullscreen);
    }

    // ==============================================================
    // Public manipulators
    // ==============================================================

    public void SetResolution(int index)
    {
        if (index < 0 || index >= resolutions.Length) return;

        Vector2Int dim = resolutions[index];
        resolutionIndex = index;
        Screen.SetResolution(dim.x, dim.y, Screen.fullScreen);
    }

    public void SetFullscreen(bool value)
    {
        Screen.fullScreen = value;
        isFullscreen = value ? 1 : 0;
    }
}
