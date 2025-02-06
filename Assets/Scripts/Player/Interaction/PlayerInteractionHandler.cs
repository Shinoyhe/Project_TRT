using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class PlayerInteractionHandler : MonoBehaviour {
  /// <summary>
  /// List of interactables that the player is in range to interact with
  /// </summary>
  private List<Interactable> accessibleInteractables;
  [SerializeField]
  /// <summary>
  /// The interactable that will be interacted with when the player tries to interact
  /// </summary>
  private Interactable highlightedInteractable;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  /// 

    [SerializeField] private PlayerInputHandler controls;
  
  void Start()
  {
    //UnityEngine.Debug.Assert(GetComponent<CharacterController>() != null, "PlayerInteractionHandler requires an attatched CharacterController&collider");
    accessibleInteractables = new List<Interactable>();
  }

  private void Update() {
    CheckHighlight();
   
    if(controls.GetInteractDown()){
        Interact();
    }
  }
  /// <summary>
  /// calls the interact function on the currently highlighted Interactable
  /// </summary>
  public void Interact() {
    if (highlightedInteractable == null) {
      return;
    }
    highlightedInteractable.GetComponent<Interactable>().Interaction();
  }
  /// <summary>
  /// Checks if we need to change which interactable is currently highlighted
  /// </summary>
  private void CheckHighlight() {
    if (highlightedInteractable == null) {
      if (accessibleInteractables.Count == 1) {
        HandlerHighlight(accessibleInteractables.ElementAt(0));
      } else if (accessibleInteractables.Count > 1) {
        HighlightNearest();
      } else if (accessibleInteractables.Count == 1) {
        return;
      }
    } else {
      if (accessibleInteractables.Count > 1) {
        HighlightNearest();
      } else if (accessibleInteractables.Count == 0) {
        UnityEngine.Debug.LogWarning("WARNING: PlayerInteractionHandler: CheckHighlight: Highlighted interactable found with empty accessible interactables list");
        HandlerUnHighlight();
      }
    }
  }
  /// <summary>
  /// sets the current highlighted Interactable to the closest interactable
  /// </summary>
  public void HighlightNearest() {
    // get the nearest interactable the player can interact with
    Interactable nearestInteractable = highlightedInteractable;
    float distanceToNearest = float.MaxValue;
    foreach(Interactable curInteractable in accessibleInteractables) {
      if (curInteractable == null) continue;
      float distanceToCurrent = Vector3.Distance(transform.position,curInteractable.transform.position);
      if( distanceToCurrent < distanceToNearest ) {
        nearestInteractable = curInteractable;
        distanceToNearest = distanceToCurrent;
      }
    }

    if (nearestInteractable == highlightedInteractable) {
      return;
    }
    HandlerHighlight(nearestInteractable);
  }
  
  /// <summary>
  /// safely highlights the given interactable, replacing the currently highlighted interactable if there is one
  /// </summary>
  /// <param name="newHighlight">The interactable to be highlighted</param>
  private void HandlerHighlight(Interactable newHighlight) {
    HandlerUnHighlight();
    highlightedInteractable = newHighlight;
    highlightedInteractable.Highlight();
  }
  /// <summary>
  /// Safely un-highlights the currently highlighted interactable
  /// </summary>
  private void HandlerUnHighlight() {
    if (highlightedInteractable == null) {
      return;
    }
    highlightedInteractable.UnHighlight();
    highlightedInteractable = null;
  }
  /// <summary>
  /// adds an interactable to the list of accessible interactables
  /// </summary>
  /// <param name="newInteractable">The Interactable to be added to the list</param>
  /// <returns>Returns true if interactable successfully added, false if not</returns>
  private bool AddAccesibleInteractable(Interactable newInteractable) {
    if (newInteractable == null) {
      UnityEngine.Debug.LogError("ERROR: PlayerInteractionHandler: AddAccesibleInteractable: Cannot add null item");
      return false;
    }
    if (accessibleInteractables.Contains(newInteractable)) {
      UnityEngine.Debug.LogWarning("WARNING: PlayerInteractionHandler: AddAccesibleInteractable: Tried to add duplicate interactable");
      return false;
    }
    accessibleInteractables.Add(newInteractable);
    return true;
  }
  /// <summary>
  /// removes an interactable from the list of accessible interactables
  /// </summary>
  /// <param name="markedInteractable"> The Interactable to be removed from the list</param>
  /// <returns>returns true if succesfully removed, and false if there was nothing to remove</returns>
  private bool RemoveAccesibleInteractable(Interactable markedInteractable) {
    if (markedInteractable == null) {
      UnityEngine.Debug.LogError("ERROR: PlayerInteractionHandler: RemoveAccesibleInteractable: Cannot remove null item");
      return false;
    }
    if (!accessibleInteractables.Contains(markedInteractable)) {
      UnityEngine.Debug.LogWarning("WARNING: PlayerInteractionHandler: RemoveAccesibleInteractable: interactable not found in list");
      return false;
    }
    accessibleInteractables.Remove(markedInteractable);
    return true;
  }

  private void OnTriggerEnter(Collider other) {
    Interactable otherInteractable = other.GetComponent<Interactable>();
    if (otherInteractable != null) {
      AddAccesibleInteractable(otherInteractable);
    }
  }  
  private void OnTriggerExit(Collider other) {
    Interactable otherInteractable = other.GetComponent<Interactable>();
    if (otherInteractable != null && accessibleInteractables.Contains(otherInteractable)) {
      if (otherInteractable == highlightedInteractable) {
        HandlerUnHighlight();
      }
      RemoveAccesibleInteractable(otherInteractable);
    }
  } 
}