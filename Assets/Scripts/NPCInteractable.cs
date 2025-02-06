using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractable : Interactable
{
    [SerializeField] private TextAsset npcConversation;

    void Start()
    {
        
    }

    public override void Highlight()
    {
        // TODO: Add Highlight Shader
    }

    public override void UnHighlight()
    {
        // TODO: Remove Highlight Shader
    }

    public override void Interaction()
    {
        DialogueManager.Instance.StartConversation(npcConversation,this.transform.position);
    }
}
