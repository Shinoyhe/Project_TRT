using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
  private void Start() {
    UnityEngine.Debug.Assert(GetComponent<Collider>() != null, "Interactable requires an attatched collider");
    UnityEngine.Debug.Assert(GetComponent<Collider>().isTrigger, "Interactable requires an attatched Trigger collider");
  }
  /// <summary>
  /// Called when the player can interact with this interactable
  /// </summary>
  public abstract void Highlight();
  /// <summary>
  /// Called when the player stops being able to interact with this interactable
  /// </summary>
  public abstract void UnHighlight();
  /// <summary>
  /// Called when the player interacts with this interactable
  /// </summary>
  public abstract void Interaction();
}
