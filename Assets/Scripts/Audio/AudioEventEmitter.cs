using System;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class AudioEventEmitter : MonoBehaviour
{
    [SerializeField] private AudioEvent audioEvent;
    private GameObject attenuationPoint;

    private void Start()
    {
        attenuationPoint = this.gameObject;
        Play();
    }

    #region Play/Pause/Stop
    public void Play()
    {
        audioEvent.Play(attenuationPoint);
    }

    public void Pause()
    {
        audioEvent.Pause();
    }

    public void Resume()
    {
        audioEvent.Resume();
    }

    public void Stop()
    {
        audioEvent.Stop();
    }
    #endregion
}
