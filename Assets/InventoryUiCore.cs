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

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private List<GameObject> _currentCardInstances = new List<GameObject>();
    private float _lastUpdateTime = 0f;
    private bool _firstUpdate = false;

    #endregion

    #region ======== [ INIT METHODS ] ========

    // Start is called before the first frame update
    private void OnEnable()
    {
        OnUiUpdate();
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    private void OnUiUpdate() {

        bool outOfDateInformation = Mathf.Abs(_lastUpdateTime - GameManager.Inventory.inventoryLastUpdateTime) > 0.025f;

        if (!outOfDateInformation && _firstUpdate == true) {
            return;
        }

        _lastUpdateTime = GameManager.Inventory.inventoryLastUpdateTime;
        _firstUpdate = true;

        List<InventoryCardData> dataForAllCards = GameManager.Inventory.GetDatas();
        PopulateGrids(ref dataForAllCards);        
    }

    private void ClearGrids() {
        foreach(GameObject card in _currentCardInstances) {
            Destroy(card);
        }

        _currentCardInstances.Clear();
    }

    private void PopulateGrids(ref List<InventoryCardData> dataForAllCards) {

        // Clear Grid!
        ClearGrids();

        // Go through each card data X
        foreach (InventoryCardData card in dataForAllCards) {

            // Find what grid to add card to.

            Transform cardGridTransform = null;

            switch (card.Type) {
                case GameEnums.CardTypes.INFO:
                    cardGridTransform = InfoCardsGrid.transform;
                    break;
                case GameEnums.CardTypes.ITEM:
                    cardGridTransform = ItemGrid.transform;
                    break;
            }

            if(cardGridTransform == null) {
                Debug.LogError("Card has no assigned parent when populating UI grid.");
            }

            // Create card
            GameObject newCard = Instantiate(InventoryCardPrefab, cardGridTransform);

            // Pair card with data
            newCard.GetComponent<InventoryCardObject>().SetData(card);

            // Add card to our tracked instances
            _currentCardInstances.Add(newCard);
        }
    }

    #endregion
}
