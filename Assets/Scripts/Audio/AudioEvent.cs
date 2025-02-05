/////////////////////////////////
// Autumn Moulios
// Last Updated 29 January 2025
// 
// AudioEvent.cs
// Made for Wwise Unity Integration 
//
/////////////////////////////////

using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[Serializable]
public class TriggerEvents
{
    public AK.Wwise.Event startEvent;
    public AK.Wwise.Event pauseEvent;
    public AK.Wwise.Event resumeEvent;
    public AK.Wwise.Event stopEvent;
}


[Serializable]
public class AudioEvent
{
    public string name;

    // State Trackers
    private bool isPlaying = false;
    public bool playOnInit = false;

    [SerializeField] private TriggerEvents _events;
    private GameObject attenuationPoint;

    //private EVENT_CALLBACK _eventCallback;
    private Dictionary<AkCallbackType, UnityEvent> _callbackHandler;

    // Serialized callback references to appear in inspector, added to _callbackHandler on init.
    [Serializable, StructLayout(LayoutKind.Sequential)]
    class AudioEventCallbacks
    {
        public UnityEvent onStart;
        public UnityEvent onRestart;
        public UnityEvent onCompleted;
        public UnityEvent onBeat;
        public UnityEvent onMarker;
    }
    [SerializeField] private AudioEventCallbacks _callbacks;
    GCHandle callbackHandle;

    #region Constructors/Destructors
    //public AudioEvent()
    //{
        //InitCallbackHandler();
        //PinCallbackData();
        //_action = new Action(Test);
    //}

    public AudioEvent(GameObject attenuationPoint)
    {
        this.attenuationPoint = attenuationPoint;
        //InitCallbackHandler();
    }

    void OnDestroy()
    {
        _events.stopEvent.Post(attenuationPoint);
    }
    #endregion



    public string GetEventName()
    {
        return name;
    }

    #region Play/Pause/Stop
    public void Play()
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
        isPlaying = true;
    }

    public void Play(GameObject attenuationPoint)
    {
        if (_events.startEvent == null)
            return;

        //_eventInstance.setCallback(_eventCallback);
        //_eventInstance.setUserData(GCHandle.ToIntPtr(callbackHandle));
        _events.startEvent.Post(attenuationPoint);
        isPlaying = true;
    }

    public void PlayOneShot()
    {
        Play();
    }

    public void PlayOneShot(GameObject attenuationPoint)
    {
        Play(attenuationPoint);
    }

    /*
    public void Seek(int ms)
    {
        if (!_eventInstance.isValid())
            return;

        _eventInstance.setTimelinePosition(ms);
    }
    */

    public void Pause()
    {
        if (_events.pauseEvent == null)
            return;

        _events.pauseEvent.Post(attenuationPoint);
        isPlaying = false;
    }

    public void Resume()
    {
        if (_events.resumeEvent == null)
            return;

        _events.resumeEvent.Post(attenuationPoint);
        isPlaying = true;
    }

    public void Fadeout()
    {
        // There's probably a way to fade out with a separate Wwise event.
        Stop();
    }

    public void Stop()
    {
        if (_events.stopEvent == null)
            return;

        _events.stopEvent.Post(attenuationPoint);
        isPlaying = false;
    }

    public bool Playing
    {
        get
        {
            return isPlaying;
        }
    }
    #endregion

    #region Callbacks

    private void InitCallbackHandler()
    {
        _callbackHandler = new Dictionary<AkCallbackType, UnityEvent>();
        //_eventCallback = new EVENT_CALLBACK(Callback);

        AK.Wwise.Bank bank;

        // Initialize UnityEvents and add them to the callback dictionary.
        _callbacks.onStart = new();
        _callbackHandler[AkCallbackType.AK_MusicPlayStarted] = _callbacks.onStart;
        _callbacks.onRestart = new();
        _callbackHandler[AkCallbackType.AK_MusicSyncEntry] = _callbacks.onRestart;
        _callbacks.onCompleted = new();
        _callbackHandler[AkCallbackType.AK_EndOfEvent] = _callbacks.onCompleted;
        _callbacks.onBeat = new();
        _callbackHandler[AkCallbackType.AK_MusicSyncBeat] = _callbacks.onBeat;
        _callbacks.onMarker = new();
        _callbackHandler[AkCallbackType.AK_MusicSyncUserCue] = _callbacks.onMarker;
    }

    private void PinCallbackData()
    {
        callbackHandle = GCHandle.Alloc(_callbackHandler, GCHandleType.Pinned);
    }

    #region Abstracted Callback Functions
    public void OnStart(UnityAction callback)
    {
        SetCallback(AkCallbackType.AK_MusicPlayStarted, callback);
    }

    public void OnRestart(UnityAction callback)
    {
        SetCallback(AkCallbackType.AK_MusicSyncEntry, callback);
    }

    public void OnComplete(UnityAction callback)
    {
        SetCallback(AkCallbackType.AK_EndOfEvent, callback);
    }

    public void OnBeat(UnityAction callback)
    {
        SetCallback(AkCallbackType.AK_MusicSyncBeat, callback);
    }

    public void OnMarker(UnityAction callback)
    {
        SetCallback(AkCallbackType.AK_MusicSyncUserCue, callback);
    }
    #endregion

    public void SetCallback(AkCallbackType type, UnityAction callback)
    {
        Debug.Log("Setting Callback");
        _callbackHandler.TryGetValue(type, out UnityEvent callbackEvent);
        if (callbackEvent == null)
        {
            _callbackHandler.Add(type, new UnityEvent());
        }

        _callbackHandler[type].AddListener(callback);
    }

    /*
    private FMOD.RESULT Callback(AkCallbackType type, IntPtr _event, IntPtr parameters)
    {
        Debug.Log("Callback Type: " + type);
        //_callbackHandler.TryGetValue(type, out UnityEvent callbackEvent);
        //if (callbackEvent == null)
        //{
        //    return FMOD.RESULT.OK;
        //}

        Debug.Log("Invoking callbacks...");
        //_callbackHandler[type].Invoke();
        //_action?.Invoke();
        Debug.Log("Success!");
        return FMOD.RESULT.OK;
    }
    */
    #endregion
}
