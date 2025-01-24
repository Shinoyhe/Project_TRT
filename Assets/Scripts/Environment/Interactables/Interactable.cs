using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
  [SerializeField]
  private static Vector3 interactionBoxDimensions = new Vector3(3,3,3);
  private BoxCollider interactionBox;
  /// <summary>
  /// Start is called on the frame when a script is enabled just before
  /// any of the Update methods is called the first time.
  /// </summary>
  void Start()
  {
      interactionBox = gameObject.AddComponent<BoxCollider>();
      interactionBox.size = interactionBoxDimensions;
      interactionBox.isTrigger = true;
  }
  /// <summary>
  /// Abstract method called by PlayerInteractionHandler to trigger this interaction
  /// </summary>
  public abstract void Interact();
  
  private void OnCollisionEnter(Collision other) {
    
  }
  
  /// <summary>
  /// Callback to draw gizmos only if the object is selected.
  /// </summary>
  void OnDrawGizmosSelected()
  {
      Gizmos.color = Color.yellow;
      Gizmos.DrawCube(transform.position, interactionBoxDimensions);
  }
}
