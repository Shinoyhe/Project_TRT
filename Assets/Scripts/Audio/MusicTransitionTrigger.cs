using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A unique music management tool that triggers the music manager to a new state upon the player entering its trigger colldier
/// </summary>
public class MusicTransitionTrigger : MonoBehaviour
{
    [SerializeField]
    private string newMusicStateName;

    [SerializeField]
    private bool isPersistentChange; // Determines whether or not the previous music selection should play upon leaving the trigger collider

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MusicActionsManager.ChangeMusicState(newMusicStateName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !isPersistentChange)
        {
            MusicActionsManager.ChangeToPreviousMusicState();
        }
    }
}
