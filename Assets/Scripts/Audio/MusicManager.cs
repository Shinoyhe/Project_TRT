using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


/// <summary>
/// Manages System Actions used by the Music Manager to allow anonymous signals to be sent from other scripts.
/// </summary>
public static class MusicActionsManager
{
    public static event Action<string> OnStateChanged;

    /// <summary>
    /// Public function for other scripts to use to invoke a state change.
    /// </summary>
    public static void ChangeMusicState(string newState)
    {
        OnStateChanged?.Invoke(newState); // Notify all subscribers
    }
}

/// <summary>
/// Custom data structure to assign a string name to a Wwise event in the inspector.
/// </summary>
[System.Serializable]
public class MusicStateEventPair
{
    public string stateName;
    public AK.Wwise.Event musicEvent;
}


/// <summary>
/// Specifically manages a Music Switch Container in Wwise and triggers Wwise events to change music functionality.
/// </summary>
[Serializable]
public class MusicManager : MonoBehaviour
{
    // Serialize a list of all state trigger events
    [SerializeField] private TriggerEvents _events;

    // Change this to a List of MusicStateEventPair
    [SerializeField] private List<MusicStateEventPair> musicStateEventsList;

    private Dictionary<string, AK.Wwise.Event> musicStateEventsDict = new Dictionary<string, AK.Wwise.Event>();


    private void Start()
    {
        InitializeAudioEventsDictionary();
    }

    /// <summary>
    /// USes the musicStateEventsList populated from the inspector and converts it to a dictionary.
    /// </summary>
    private void InitializeAudioEventsDictionary()
    {
        // Populate the state event dictionary from the given serialized list.
        foreach (var pair in musicStateEventsList)
        {
            musicStateEventsDict.Add(pair.stateName, pair.musicEvent);
        }

        // I may create a "playOnInit" variable to avoid hardcoding playing music on initialization.
        SetMusicState("PlayerSpawn");
        PlayMusic(gameObject);
    }

    private void OnEnable()
    {
        MusicActionsManager.OnStateChanged += SetMusicState;
    }

    private void OnDisable()
    {
        Debug.Log("Called disable :(");
        MusicActionsManager.OnStateChanged -= SetMusicState;
    }

    private void OnDestroy()
    {
        Debug.Log("Called destroy :)");
        StopCurrentMusic();
        MusicActionsManager.OnStateChanged -= SetMusicState;
    }

    #region Music Management
    public void PlayMusic(GameObject attenuationPoint)
    {
        if (_events.startEvent == null)
            return;

        if (attenuationPoint == null)
        {
            Debug.LogError("No Attenuation Point set for event " + name);
            return;
        }

        //_eventInstance.setCallback(_eventCallback);
        _events.startEvent.Post(attenuationPoint);
        //isPlaying = true;
    }

    public void PauseCurrentMusic(GameObject attenuationPoint)
    {
        if (_events.pauseEvent == null)
            return;

        if (attenuationPoint == null)
        {
            Debug.LogError("No Attenuation Point set for event " + name);
            return;
        }

        //_eventInstance.setCallback(_eventCallback);
        _events.pauseEvent.Post(attenuationPoint);
        //isPlaying = true;
    }

    public void ResumeCurrentMusic(GameObject attenuationPoint)
    {
        if (_events.resumeEvent == null)
            return;

        if (attenuationPoint == null)
        {
            Debug.LogError("No Attenuation Point set for event " + name);
            return;
        }

        //_eventInstance.setCallback(_eventCallback);
        _events.resumeEvent.Post(attenuationPoint);
        //isPlaying = true;
    }

    public void StopCurrentMusic()
    {
        if (_events.stopEvent == null)
            return;

        //_eventInstance.setCallback(_eventCallback);
        _events.stopEvent.Post(gameObject); // I don't wanna use an
        //isPlaying = true;
    }

    #endregion

    #region State Management
    /// <summary>
    /// Sets current state of music container to one specified by the state name parameter.
    /// </summary>
    /// <param name="stateName"> String value of desired state to transition to. Should be the same name as found in Wwise project. </param>
    public void SetMusicState(string stateName)
    {
        if (musicStateEventsDict.ContainsKey(stateName))
        {
            musicStateEventsDict[stateName].Post(gameObject);
        }
    }

    #endregion
}
