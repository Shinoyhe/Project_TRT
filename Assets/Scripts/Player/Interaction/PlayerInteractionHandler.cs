using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;

public class PlayerInteractionHandler : MonoBehaviour {

    /// <summary>
    /// The interactable that will be interacted with when the player tries to interact
    /// </summary>
    [SerializeField] private InteractionIcon interactionIcon;

    /// <summary>
    /// The interactable that will be interacted with when the player tries to interact
    /// </summary>
    [SerializeField] private Interactable highlightedInteractable;

    /// <summary>
    /// List of interactables that the player is in range to interact with
    /// </summary>
    [ReadOnly] [SerializeField] private List<Interactable> _accessibleInteractables;

    /// <summary>
    /// Whether or not the player can currently interact with Interactables.
    /// </summary>
    [ReadOnly] [SerializeField] private bool _canInteract = true;

    /// <summary>
    /// SFX that plays when interaction icon appears
    /// </summary>
    [SerializeField] private AudioEvent interactionIconEnterSFX;
    /// <summary>
    /// SFX that plays when interaction icon disappears
    /// </summary>
    [SerializeField] private AudioEvent interactionIconLeaveSFX;

    void Start()
    {
        //UnityEngine.Debug.Assert(GetComponent<CharacterController>() != null,
        //                         "PlayerInteractionHandler requires an attatched CharacterController&collider");
        _accessibleInteractables = new List<Interactable>();
    }

    private void Update() 
    {
        CheckHighlight();

        if (GameManager.PlayerInput.GetPrimaryTriggerDown() || 
            GameManager.PlayerInput.GetAffirmDown() || GameManager.PlayerInput.GetClickDown()) {
            Interact();
        }
    }

    /// <summary>
    /// Public setter for _canInteract.
    /// </summary>
    /// <param name="canInteract">bool - whether or not the player can use Interactables.</param>
    public void SetCanInteract(bool canInteract)
    {
        _canInteract = canInteract;
    }

    /// <summary>
    /// calls the interact function on the currently highlighted Interactable
    /// </summary>
    public void Interact() 
    {
        if (!_canInteract || highlightedInteractable == null) {
            return;
        }

        highlightedInteractable.GetComponent<Interactable>().Interaction();
    }

    /// <summary>
    /// Checks if we need to change which interactable is currently highlighted
    /// </summary>
    private void CheckHighlight() 
    {
        // Remove destroyed from _accessibleInteractables
        _accessibleInteractables.RemoveAll(item => item == null);

        if (highlightedInteractable == null) 
        {
            if (_accessibleInteractables.Count == 1) 
            {
                HandlerHighlight(_accessibleInteractables.ElementAt(0));
            } 
            else if (_accessibleInteractables.Count > 1) 
            {
                HighlightNearest();
            }
            else if (_accessibleInteractables.Count == 0) 
            {
                interactionIcon.Hide();
            }
        }
        else 
        {
            if (_accessibleInteractables.Count > 1) 
            {
                HighlightNearest();
            } 
            else if (_accessibleInteractables.Count == 0) 
            {
                Debug.LogWarning("WARNING: PlayerInteractionHandler: CheckHighlight: Highlighted " +
                                 "interactable found with empty accessible interactables list");
                HandlerUnHighlight();
            }
        }
    }

    /// <summary>
    /// sets the current highlighted Interactable to the closest interactable
    /// </summary>
    public void HighlightNearest() 
    {
        // get the nearest interactable the player can interact with
        Interactable nearestInteractable = highlightedInteractable;
        float distanceToNearest = float.MaxValue;
        foreach(Interactable curInteractable in _accessibleInteractables) 
        {
            if (curInteractable == null) continue;
            float distanceToCurrent = Vector3.Distance(transform.position, 
                                                       curInteractable.transform.position);
                                                       
            if (distanceToCurrent < distanceToNearest) 
            {
                nearestInteractable = curInteractable;
                distanceToNearest = distanceToCurrent;
            }
        }

        if (nearestInteractable == highlightedInteractable) 
        {
            return;
        }

        HandlerHighlight(nearestInteractable);
    }

    /// <summary>
    /// safely highlights the given interactable, replacing the currently highlighted interactable if there is one
    /// </summary>
    /// <param name="newHighlight">The interactable to be highlighted</param>
    private void HandlerHighlight(Interactable newHighlight) 
    {
        HandlerUnHighlight();
        highlightedInteractable = newHighlight;
        highlightedInteractable.Highlight();
        ShowInteractIcon(newHighlight);

        interactionIconEnterSFX.Play(gameObject);
    }

    /// <summary>
    /// Safely un-highlights the currently highlighted interactable
    /// </summary>
    private void HandlerUnHighlight() 
    {
        if (highlightedInteractable == null) 
        {
            return;
        }

        highlightedInteractable.UnHighlight();
        interactionIcon.Hide();
        highlightedInteractable = null;

        interactionIconLeaveSFX.Play(gameObject);
    }

    private void ShowInteractIcon(Interactable newHighlight)
    {
        if (newHighlight == null)
        {
            interactionIcon.Hide();
        }

        if (newHighlight.UseTransform)
        {
            interactionIcon.Show(newHighlight.IconTransformPosition);
        }
        else
        {
            interactionIcon.Show(newHighlight.IconLocalPosition + newHighlight.transform.position);
        }
    }

    /// <summary>
    /// adds an interactable to the list of accessible interactables
    /// </summary>
    /// <param name="newInteractable">The Interactable to be added to the list</param>
    /// <returns>Returns true if interactable successfully added, false if not</returns>
    private bool AddAccessibleInteractable(Interactable newInteractable) 
    {
        if (newInteractable == null) 
        {
            Debug.LogError("ERROR: PlayerInteractionHandler: AddAccessibleInteractable: Cannot " +
                           "add null item");
            return false;
        }

        if (_accessibleInteractables.Contains(newInteractable)) 
        {
            Debug.LogWarning("WARNING: PlayerInteractionHandler: AddAccessibleInteractable: " +
                             "Tried to add duplicate interactable");
            return false;
        }

        _accessibleInteractables.Add(newInteractable);
        return true;
    }

    /// <summary>
    /// removes an interactable from the list of accessible interactables
    /// </summary>
    /// <param name="markedInteractable"> The Interactable to be removed from the list</param>
    /// <returns>returns true if succesfully removed, and false if there was nothing to remove</returns>
    private bool RemoveAccesibleInteractable(Interactable markedInteractable) 
    {
        if (markedInteractable == null) 
        {
            Debug.LogError("ERROR: PlayerInteractionHandler: RemoveAccesibleInteractable: Cannot remove null item");
            return false;
        }
        if (!_accessibleInteractables.Contains(markedInteractable)) {
            Debug.LogWarning("WARNING: PlayerInteractionHandler: RemoveAccesibleInteractable: interactable not found in list");
            return false;
        }

        _accessibleInteractables.Remove(markedInteractable);
        return true;
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other) 
        {
            if (other.TryGetComponent<Interactable>(out var otherInteractable)) 
            {
                AddAccessibleInteractable(otherInteractable);
            }
        }
    }

    private void OnTriggerExit(Collider other) 
    {
        Interactable otherInteractable = other.GetComponent<Interactable>();

        if ((otherInteractable != null) && _accessibleInteractables.Contains(otherInteractable)) 
        {
            if (otherInteractable == highlightedInteractable) 
            {
                HandlerUnHighlight();
            }
            RemoveAccesibleInteractable(otherInteractable);
        }
    } 
}