using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractable : Interactable
{
  public override void Interaction() {

    Debug.Log("Interaction called on: " + gameObject.name);
  }
  public override void Highlight() {
    Debug.Log("Highlighted " + gameObject.name);
  }
  public override void UnHighlight() {
    Debug.Log("UnHighlighted " + gameObject.name);
  }
}
