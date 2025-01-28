using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerInteractionHandler : MonoBehaviour {
  
  [SerializeField]
  private List<IInteractable> accessibleInteractables;
  private IInteractable highlightedInteractable;

  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start()
  {
    Debug.Assert(GetComponent<Rigidbody>() != null, "PlayerInteractionHandler requires an attatched rigidbody&collider");
    accessibleInteractables = new List<IInteractable>();
  }
  /// <summary>
  /// calls the interact function on the currently highlighted Interactable
  /// </summary>
  public void Interact() {

  }
  /// <summary>
  /// adds an interactable to the list of accessible interactables
  /// </summary>
  /// <param name="newInteractable"></param>
  /// <returns>Returns true if interactable successfully added, false if not</returns>
  public bool AddAccesibleInteractable(IInteractable newInteractable) {
    if (newInteractable == null) {
      Debug.LogError("ERROR: PlayerInteractionHandler: AddAccesibleInteractable: Cannot add null item");
      return false;
    }
    if (accessibleInteractables.Contains(newInteractable)) {
      Debug.LogWarning("WARNING: PlayerInteractionHandler: AddAccesibleInteractable: Tried to add duplicate interactable");
      return false;
    }
    accessibleInteractables.Add(newInteractable);
    Debug.Log("added interactable: " + newInteractable);
    return true;
  }
  /// <summary>
  /// removes an interactable from the list of accessible interactables
  /// </summary>
  /// <param name="markedInteractable"></param>
  /// <returns>returns true if succesfully removed, and false if there was nothing to remove</returns>
  public bool RemoveAccesibleInteractable(IInteractable markedInteractable) {
    if (markedInteractable == null) {
      Debug.LogError("ERROR: PlayerInteractionHandler: RemoveAccesibleInteractable: Cannot remove null item");
      return false;
    }
    if (!accessibleInteractables.Contains(markedInteractable)) {
      Debug.LogWarning("WARNING: PlayerInteractionHandler: RemoveAccesibleInteractable: interactable not found in list");
      return false;
    }
    Debug.Log("removed interactable: " + markedInteractable);
    accessibleInteractables.Remove(markedInteractable);
    return true;
  }

  private void OnTriggerEnter(Collider other) {
    Debug.Log("hit somthin");
    IInteractable otherInteractable = other.GetComponent<IInteractable>();
    if (otherInteractable != null) {
      AddAccesibleInteractable(otherInteractable);
    }
  }  private void OnTriggerExit(Collider other) {
    IInteractable otherInteractable = other.GetComponent<IInteractable>();
    if (otherInteractable != null && accessibleInteractables.Contains(otherInteractable)) {
      RemoveAccesibleInteractable(otherInteractable);
    }
  }
  
}
