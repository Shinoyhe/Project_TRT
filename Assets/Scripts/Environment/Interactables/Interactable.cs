using UnityEngine;

/// <summary>
/// Abstract class for any world objects the player can interact with.
/// holds abstract methods for defining what happens on an interation
/// and what should happen when a player is in position to interact with
/// the interactable.
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    private void Start() 
    {
        Debug.Assert(GetComponent<Collider>() != null, "Interactable requires an attatched collider.");
        Debug.Assert(GetComponent<Collider>().isTrigger, "Interactable requires an attatched Trigger collider.");
    }

    /// <summary>
    /// Called when the player is positioned to interact with this interactable.
    /// </summary>
    public abstract void Highlight();

    /// <summary>
    /// Called when the player stops being able to interact with this interactable.
    /// </summary>
    public abstract void UnHighlight();

    /// <summary>
    /// Called when the player interacts with this interactable.
    /// </summary>
    public abstract void Interaction();
}
