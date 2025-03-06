using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialNPC : MonoBehaviour
{
    [SerializeField] private NpcInteractable npcInteractable;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance != null)
        {

        }

        if (npcInteractable == null) { return; }
        npcInteractable.Interaction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
