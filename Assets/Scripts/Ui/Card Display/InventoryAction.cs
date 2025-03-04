using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryAction : MonoBehaviour {
    public struct ActionContext {
        public InventoryCardData cardData;
    }

    public abstract void ActionOnClick(ActionContext context);
}
