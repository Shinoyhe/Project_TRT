using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoScrollTrigger : MonoBehaviour, ISelectHandler {

    public InventoryCardObject parent;

   
    public void OnSelect(BaseEventData eventData) {
        parent.OnSelect(eventData);
    }


}
