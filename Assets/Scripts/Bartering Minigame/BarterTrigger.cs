using UnityEngine;

public class BarterTrigger : MonoBehaviour
{
    public GameObject BarterContainer;

    // Update is called once per frame
    void Update()
    {
        if (GameManager.UiInput.GetDebugDown()) {
           Instantiate(BarterContainer, Vector3.zero,Quaternion.identity);
        }
    }
}
