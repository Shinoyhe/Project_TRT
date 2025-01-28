using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
  
  /// <summary>
  /// Called when the player can interact with this interactable
  /// </summary>
  public void Highlight();
  /// <summary>
  /// Called when the player stops being able to interact with this interactable
  /// </summary>
  public void UnHighlight();
  /// <summary>
  /// Called when the player interacts with this interactable
  /// </summary>
  public void Interaction();
}
