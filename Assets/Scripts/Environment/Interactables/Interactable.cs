using UnityEngine;
using NaughtyAttributes;

/// <summary>
/// Abstract class for any world objects the player can interact with.
/// holds abstract methods for defining what happens on an interation
/// and what should happen when a player is in position to interact with
/// the interactable.
/// </summary>
public abstract class Interactable : MonoBehaviour
{


    [BoxGroup("Interact Icon Position")]
    public bool UseTransform = false;
    [BoxGroup("Interact Icon Position")] [ShowIf("UseTransform")]
    public Transform IconTransformPosition;
    [BoxGroup("Interact Icon Position")] [HideIf("UseTransform")]
    public Vector3 IconLocalPosition = Vector3.up;

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
