using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardInspectAction : InventoryAction {

    public InventoryCardObject InspectCard;

    bool inspecting = false;
    InventoryCardData currentData = null;

    private void Start() {
        InspectCard.gameObject.SetActive(false);
    }

    public override void ActionOnClick(ActionContext context) {

        if(context.cardData == null) {
            return;
        }

        bool NewInformation = currentData == null || currentData != context.cardData;

        if (NewInformation == false) {
            inspecting = false;
            currentData = null;
            InspectCard.gameObject.SetActive(false);
        }

        if(NewInformation == true) {
            inspecting = true;
            currentData = context.cardData;
            InspectCard.gameObject.SetActive(true);
            InspectCard.SetData(currentData);
        }
    }

}
