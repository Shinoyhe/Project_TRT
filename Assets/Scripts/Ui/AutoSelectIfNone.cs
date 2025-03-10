using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AutoSelectIfNone : MonoBehaviour {

    GameObject lastSelected;

    // Update is called once per frame
    void Update() {
        Vector3 Input = GameManager.PlayerInput.GetControlInput();

        if (Input.magnitude > 0.02f) {
            if (EventSystem.current.currentSelectedGameObject == null) {
                EventSystem.current.SetSelectedGameObject(lastSelected);
            }
        }


        if (EventSystem.current.currentSelectedGameObject != null) {
            lastSelected = EventSystem.current.currentSelectedGameObject;
            Debug.Log("Selected: " + EventSystem.current.currentSelectedGameObject);
            if (EventSystem.current.currentSelectedGameObject.activeInHierarchy == false) {
                EventSystem.current.SetSelectedGameObject(null);
            }
            //if(EventSystem.current.currentSelectedGameObject.)
        }


    }
}
