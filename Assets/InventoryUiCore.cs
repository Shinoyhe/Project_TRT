using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUiCore : MonoBehaviour
{
    #region ======== [ OBJECT REFERENCES ] ========

    public GridLayoutGroup InfoCardsGrid;
    public GridLayoutGroup ItemGrid;
    public GameObject InventoryCardPrefab;
    public InventoryCardObject inspectCard;

    public int InfoCardMaxCount = 10;
    public int ItemCardMaxCount = 10;

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private List<GameObject> _currentInfoCardInstances = new List<GameObject>();
    private List<GameObject> _currentItemCardInstances = new List<GameObject>();

    private float _lastUpdateTime = 0f;
    private bool _firstUpdateNotDone = true;

    #endregion

    #region ======== [ INIT METHODS ] ========

    // Start is called before the first frame update
    private void OnEnable()
    {
        Debug.Log("On Enable!");
        inspectCard.gameObject.SetActive(false);

        if (_firstUpdateNotDone) {
            OnFirstUpdate();
        }

        OnUiUpdate();
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========
    
    private void OnFirstUpdate() {

        for (int i = 0; i < InfoCardMaxCount; i++) {

            // Create a new card instance
            GameObject newCard = Instantiate(InventoryCardPrefab, InfoCardsGrid.transform);

            // Pair card with data
            newCard.GetComponent<InventoryCardObject>().inspectCard = inspectCard;
            newCard.GetComponent<InventoryCardObject>().SetCardToEmpty();

            // Add card to our tracked instances
            _currentInfoCardInstances.Add(newCard);
        }
         
        for (int i = 0; i < ItemCardMaxCount; i++) {

            // Create a new card instance
            GameObject newCard = Instantiate(InventoryCardPrefab, ItemGrid.transform);

            // Pair card with data
            newCard.GetComponent<InventoryCardObject>().inspectCard = inspectCard;
            newCard.GetComponent<InventoryCardObject>().SetCardToEmpty();

            // Add card to our tracked instances
            _currentItemCardInstances.Add(newCard);
        }

        List<InventoryCardData> dataForAllCards = GameManager.Inventory.GetDatas();
        PopulateGrids(ref dataForAllCards);

        _firstUpdateNotDone = false;
    }

    private void OnUiUpdate() {

        bool outOfDateInformation = Mathf.Abs(_lastUpdateTime - GameManager.Inventory.inventoryLastUpdateTime) > 0.025f;

        if (!outOfDateInformation) {
            return;
        }

        _lastUpdateTime = GameManager.Inventory.inventoryLastUpdateTime;

        List<InventoryCardData> dataForAllCards = GameManager.Inventory.GetDatas();
        PopulateGrids(ref dataForAllCards);        
    }

    private void ClearGrid() {
        foreach(GameObject card in _currentItemCardInstances) {
            card.GetComponent<InventoryCardObject>().SetCardToEmpty();
        }

        foreach (GameObject card in _currentInfoCardInstances) {
            card.GetComponent<InventoryCardObject>().SetCardToEmpty();
        }
    }

    private void PopulateGrids(ref List<InventoryCardData> dataForAllCards) {

        // Clear Grid!
        ClearGrid();

        int currentAddedInfo = 0;
        int currentAddedItem = 0;

        // Go through each card data X
        foreach (InventoryCardData card in dataForAllCards) {

            // Find what grid to add card to.

            switch (card.Type) {
                case GameEnums.CardTypes.INFO:

                    _currentInfoCardInstances[currentAddedInfo].GetComponent<InventoryCardObject>().SetData(card);
                    currentAddedInfo += 1;

                    break;
                case GameEnums.CardTypes.ITEM:

                    _currentItemCardInstances[currentAddedItem].GetComponent<InventoryCardObject>().SetData(card);
                    currentAddedItem += 1;
                    break;
            }
        }
    }

    #endregion
}
