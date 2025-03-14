using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialNPC : MonoBehaviour
{
    [SerializeField] private FadeToBlack fadeToBlack;
    [SerializeField] private NpcInteractable npcInteractable;
    [SerializeField] private List<InventoryCardData> postTutorialCards;

    // Start is called before the first frame update
    void Start()
    {

        // if (npcInteractable == null) { return; }
        // npcInteractable.Interaction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetUpTutorial(){
        StartCoroutine(TransitionToGame());
    }

    private IEnumerator TransitionToGame()
    {
        GameManager.Player.Movement.SetCanMove(false);
        GameManager.Player.InteractionHandler.SetCanInteract(false);
        foreach (InventoryCardData inventoryCardData in postTutorialCards)
        {
            if (inventoryCardData != null)
            {
                GameManager.Inventory.AddCard(inventoryCardData);
            }
        }

        yield return fadeToBlack.StartFadeIn();
        
        GameManager.Player.Movement.SetCanMove(true);
        GameManager.Player.InteractionHandler.SetCanInteract(true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
