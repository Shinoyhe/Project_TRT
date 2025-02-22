using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIAudioSlider : MonoBehaviour
{
    public AK.Wwise.RTPC bus;
    private string busName;

    [SerializeField] private string displayNameOverride = "";
    [SerializeField] private GameObject nameObj;
    [SerializeField] private GameObject valueObj;

    public void Start()
    {
        busName = ParseDisplayName();
        nameObj.GetComponent<TMP_Text>().text = busName;
        InitVolume();
    }

    // Save settings to PlayerPrefs on destroy.
    private void OnDestroy()
    {
        float volume = bus.GetGlobalValue();
        PlayerPrefs.SetFloat(busName, volume);
    }

    // Display name will be the same as the FMOD Bus, unless displayNameOverride exists.
    private string ParseDisplayName()
    {
        return bus.ToString();
    }

    // Load saved volume, or default to bus volume if none is saved.
    private void InitVolume()
    {
        float volume;

        if (PlayerPrefs.HasKey(busName))
        {
            volume = PlayerPrefs.GetFloat(busName);
            bus.SetGlobalValue(volume);
        }
        else
        {
            volume = bus.GetGlobalValue();
        }
        
        GetComponent<Slider>().value = volume;
    }

    // Sliders should be 0% - 200% volume (default 100%)
    public void UpdateVolume(System.Single volume)
    {
        bus.SetGlobalValue(volume);
    }

    public void UpdateText(System.Single value)
    {
        // Convert from range 0 - 2 to range 0 - 200
        valueObj.GetComponent<TMP_Text>().text = (value * 100).ToString("f0") + "%";
    }
}
