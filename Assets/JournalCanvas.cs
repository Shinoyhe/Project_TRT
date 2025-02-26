using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class JournalCanvas : MonoBehaviour
{
    private enum TabType
    {
        NPC, Info, Item
    }


    [SerializeField] private GameObject NPCMain;
    [SerializeField] private GameObject InfoMain;
    [SerializeField] private GameObject ItemMain;

    [SerializeField] private Image icon;
    [SerializeField] private TextMeshProUGUI bio;


    private BaseTabButton _currentTabButton;
    private TabType _currentTabType;



    public void SelectTab(BaseTabButton tabButton)
    {
        if (_currentTabButton != tabButton)
        {
            _currentTabButton.Enable();
            tabButton.Disable();
            _currentTabButton = tabButton;
        }

        var tabType = tabButton switch
        {
            JournalNPCButton => TabType.NPC,
            JournalInfoButton => TabType.Info,
            JournalItemButton => TabType.Item,
            _ => TabType.NPC
        };
    }


    /// <summary>
    /// Changes which tab type is the active one.
    /// </summary>
    /// <param name="tabType"></param>
    private void SwitchTabType(TabType tabType)
    {
        if (_currentTabType == tabType) return;

        GameObject oldTab = tabType switch
        {
            TabType.NPC => NPCMain,
            TabType.Info => InfoMain,
            TabType.Item => ItemMain,
            _ => null
        };

        GameObject newTab = tabType switch
        {
            TabType.NPC => NPCMain,
            TabType.Info => InfoMain,
            TabType.Item => ItemMain,
            _ => null
        };

        oldTab?.SetActive(false);
        newTab?.SetActive(true);


    }

    /// <summary>
    /// Loads the NPC Data to be displayed
    /// </summary>
    /// <param name="npcData"></param>
    private void LoadNPC(NPCData npcData)
    {
        icon.sprite = npcData.Icon;
        bio.text = npcData.Bio;
    }


    private void LoadInfo()
    {
        
    }


    private void LoadItems()
    {
        
    }
}
