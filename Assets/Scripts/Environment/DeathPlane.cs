using UnityEngine;

public class DeathPlane : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            TimeLoopManager.ResetLoop();
        }
    }
}