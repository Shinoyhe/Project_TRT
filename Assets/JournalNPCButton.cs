using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

public class BaseTabButton : MonoBehaviour
{
    [Header("Object References")]
    [Required] [SerializeField] private JournalCanvas journal;
    public JournalCanvas.TabType tabType = JournalCanvas.TabType.NPC;
    private Button button;

    /// <summary>
    /// Switches the journal's section to about this NPC Data
    /// </summary>
    public virtual void OnPress()
    {
        journal.SelectTab(this);
    }

    /// <summary>
    /// Disables the button from being pressed
    /// </summary>
    public virtual void Disable()
    {
        button.interactable = false;
    }


    /// <summary>
    /// Enables the button to be pressed
    /// </summary>
    public virtual void Enable()
    {
        button.interactable = true;
    }


    void Awake()
    {
        button = GetComponent<Button>();
    }
}


public class JournalNPCButton : BaseTabButton
{
    [Header("Parameter")]
    [Label("NPC Data")] public NPCData npcData;
}
