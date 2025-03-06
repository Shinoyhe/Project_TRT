using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialNPC : MonoBehaviour
{
    [SerializeField] private NpcInteractable npcInteractable;
    [SerializeField] private List<InventoryCardData> postTutorialCards;

    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.BarterStarter.OnWin += TransitionToGame;
            GameManager.BarterStarter.OnLose += TransitionToGame;
        }

        if (npcInteractable == null) { return; }
        npcInteractable.Interaction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TransitionToGame()
    {
        foreach (InventoryCardData inventoryCardData in postTutorialCards)
        {
            if (inventoryCardData != null)
            {
                GameManager.Inventory.AddCard(inventoryCardData);
            }
        }

        SceneManager.LoadScene("EnvInteractables");
    }
}
