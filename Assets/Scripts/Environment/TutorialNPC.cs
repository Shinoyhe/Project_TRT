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
        if (GameManager.Instance != null)
        {
            GameManager.BarterStarter.OnWin += () => StartCoroutine(TransitionToGame());
            GameManager.BarterStarter.OnLose += () => StartCoroutine(TransitionToGame());
        }

        StartCoroutine(StartTutorial());
    }
    
    private IEnumerator StartTutorial()
    {
        yield return new WaitForSecondsRealtime(1);
        
        if (npcInteractable == null) { yield break; }
        npcInteractable.Interaction();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator TransitionToGame()
    {
        npcInteractable.enabled = false;
        foreach (InventoryCardData inventoryCardData in postTutorialCards)
        {
            if (inventoryCardData != null)
            {
                GameManager.Inventory.AddCard(inventoryCardData);
            }
        }

        yield return fadeToBlack.StartFadeIn();

        SceneManager.LoadScene("EnvInteractables");
    }
}
