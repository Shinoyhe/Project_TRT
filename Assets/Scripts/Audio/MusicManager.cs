using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private List<AudioEvent> musicEvents = new();
    [SerializeField] private List<AK.Wwise.RTPC> buses = new();
    private Dictionary<string, AudioEvent> musicEventsDict = new();
    private AudioEvent focusedMusic;
    private Stack<AudioEvent> allCurrentMusic;

    private void Start()
    {
        InitializeAudioEventsDictionary();
    }

    private void InitializeAudioEventsDictionary()
    {
        for (int i = 0; i < musicEvents.Count; ++i)
        {
            musicEventsDict[musicEvents[i].GetEventName()] = musicEvents[i];
            if (musicEvents[i].playOnInit) musicEvents[i].Play(this.gameObject); Debug.Log("hi");
        }

        Debug.Log("Here's the events: " + musicEventsDict.Values);
        
        foreach (AK.Wwise.RTPC bus in buses){
            string busName = bus.ToString().Replace("Volume", "");
            if (PlayerPrefs.HasKey(busName))
            {
                bus.SetGlobalValue(PlayerPrefs.GetFloat(busName));
            }
            else{
                bus.SetGlobalValue(50);
            }
        }
    }

    private void OnDestroy()
    {
        //focusedMusic.Free();
    }

    public AudioEvent GetMusic(string musicName)
    {
        return musicEventsDict[musicName];
    }

    #region Music Management
    public void PlayMusic(string newMusicName)
    {
        if (focusedMusic != null)
        {
            allCurrentMusic.Push(focusedMusic);
        }

        AudioEvent newMusic = GetMusic(newMusicName);
        focusedMusic = newMusic;
        focusedMusic.Play();
    }

    public void PauseCurrentMusic()
    {
        focusedMusic.Pause();
    }

    public void ResumeCurrentMusic()
    {
        focusedMusic?.Resume();
    }

    public void StopCurrentMusic()
    {
        if (focusedMusic != null)
        {
            focusedMusic.Stop();
        }
    }

    public void FadeoutCurrentMusic()
    {
        if (focusedMusic != null)
        {
            focusedMusic.Fadeout();
        }
    }

    public void SwitchMusic(AudioEvent newMusic)
    {
        StopCurrentMusic();

        focusedMusic = newMusic;
        focusedMusic.Play();
    }

    public void CrossfadeMusic(AudioEvent newMusic)
    {
        FadeoutCurrentMusic();

        focusedMusic = newMusic;
        focusedMusic.Play();
    }

    public void SwitchAfterFadeout(AudioEvent newMusic)
    {
        if (focusedMusic != null)
        {
            //focusedMusic.OnComplete(newMusic.Play);
            focusedMusic.Fadeout();
        }
    }
    #endregion

    #region Parameter Management

    #endregion
}
