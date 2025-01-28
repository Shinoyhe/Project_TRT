using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
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
    HighlightNearest();
  }
  /// <summary>
  /// calls the interact function on the currently highlighted Interactable
  /// </summary>
  public void Interact() {
    if (highlightedInteractable == null) return;
    highlightedInteractable.GetComponent<Interactable>().Interaction();
  }
  /// <summary>
  /// sets the current highlighted Interactable to the closest interactable
  /// calls IInteractable.Highlight on the the new closest interactable
  /// calls IInteractable.UnHighlight on the the old highlighted interactable
  /// </summary>
  public void HighlightNearest() {
    if (accessibleInteractables.Count == 0) return;
    Interactable nearestInteractable = highlightedInteractable;
    float distanceToNearest = float.MaxValue;

    foreach(Interactable curInteractable in accessibleInteractables) {
      float distanceToCurrent = Vector3.Distance(transform.position,curInteractable.transform.position);
      if( distanceToCurrent < distanceToNearest ) {
        nearestInteractable = curInteractable;
        distanceToNearest = distanceToCurrent;
      }
    }

    if (nearestInteractable == highlightedInteractable) return;
    if (highlightedInteractable != null) {
      highlightedInteractable.UnHighlight();
    }
    highlightedInteractable = nearestInteractable;
    highlightedInteractable.Highlight();
  }
  /// <summary>
  /// adds an interactable to the list of accessible interactables
  /// </summary>
  /// <param name="newInteractable"></param>
  /// <returns>Returns true if interactable successfully added, false if not</returns>
  public bool AddAccesibleInteractable(Interactable newInteractable) {
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
  /// <param name="markedInteractable"></param>
  /// <returns>returns true if succesfully removed, and false if there was nothing to remove</returns>
  public bool RemoveAccesibleInteractable(Interactable markedInteractable) {
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
      RemoveAccesibleInteractable(otherInteractable);
      HighlightNearest();
    }
  } 
}