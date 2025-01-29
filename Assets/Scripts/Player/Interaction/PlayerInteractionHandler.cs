using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Unity.VisualScripting.ReorderableList.Element_Adder_Menu;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

public class PlayerInteractionHandler : MonoBehaviour {
  private List<Interactable> accessibleInteractables;
  [SerializeField]
  private Interactable highlightedInteractable;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start()
  {
    UnityEngine.Debug.Assert(GetComponent<Rigidbody>() != null, "PlayerInteractionHandler requires an attatched rigidbody&collider");
    accessibleInteractables = new List<Interactable>();
  }
  private void Update() {
    CheckHighlight();
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
  /// calls IInteractable.Highlight on the the new closest interactable
  /// calls IInteractable.UnHighlight on the the old highlighted interactable
  /// </summary>
  public void HighlightNearest() {
    // get the nearest interactable the player can interact with
    Interactable nearestInteractable = highlightedInteractable;
    float distanceToNearest = float.MaxValue;
    foreach(Interactable curInteractable in accessibleInteractables) {
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
  
  private void HandlerHighlight(Interactable newHighlight) {
    HandlerUnHighlight();
    highlightedInteractable = newHighlight;
    highlightedInteractable.Highlight();
  }
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
    UnityEngine.Debug.Log("added interactable: " + newInteractable);
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
    UnityEngine.Debug.Log("removed interactable: " + markedInteractable);
    accessibleInteractables.Remove(markedInteractable);
    return true;
  }

  private void OnTriggerEnter(Collider other) {
    UnityEngine.Debug.Log("hit somthin");
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