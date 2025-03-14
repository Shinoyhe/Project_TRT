using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarterWinScreen : MonoBehaviour
{

    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;
    [SerializeField] private InventoryCardObject acceptedCardObj;
    [SerializeField] private InventoryCardObject rewardCardObj;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private TMP_Text npcName;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(bool won, InventoryCardData acceptedCard, InventoryCardData rewardCard, NPCData npc)
    {
        if (won)
        {
            winScreen.SetActive(true);
            loseScreen.SetActive(false);
        } else
        {
            winScreen.SetActive(false);
            loseScreen.SetActive(true);
        }

        acceptedCardObj.SetData(acceptedCard);
        rewardCardObj.SetData(rewardCard);

        npcPortrait.sprite = npc.Icon;
        npcName.text = npc.Name;
    }
}
