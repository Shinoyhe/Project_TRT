using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUiCore : MonoBehaviour {
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

    private bool inspecting = false;

    #endregion

    #region ======== [ INIT METHODS ] ========

    // Start is called before the first frame update
    private void OnEnable() {
        Debug.Log("On Enable!");
        inspectCard.gameObject.SetActive(false);

        if (_firstUpdateNotDone) {
            OnFirstUpdate();
        }

        CheckForUpdate();
    }

    #endregion

    #region ======== [ PUBLIC METHODS ] ========

    /// <summary>
    /// Start inspecting a card in the inventory.
    /// </summary>
    /// <param name="dataToShow">Card data to show in the inspect screen.</param>
    public void InspectCard(InventoryCardData dataToShow) {

        if (dataToShow == null) return;

        inspectCard.gameObject.SetActive(true);
        inspectCard.SetData(dataToShow);
        inspecting = true;

    }

    /// <summary>
    /// Hide the inspection popup!
    /// </summary>
    public void StopInspecting() {
        
        inspectCard.gameObject.SetActive(false);
        inspecting = false;
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// Creates the inventory grid of InventoryCardObjects to use later!
    /// Only triggers once, the first OnEnable call for this object.
    /// </summary>
    private void OnFirstUpdate() {

        // Instance Info Cards
        for (int i = 0; i < InfoCardMaxCount; i++) {
            GameObject newCard = InstanceNewCard(InfoCardsGrid.transform);
            _currentInfoCardInstances.Add(newCard);
        }

        // Instance Item Cards
        for (int i = 0; i < ItemCardMaxCount; i++) {
            GameObject newCard = InstanceNewCard(ItemGrid.transform);
            _currentItemCardInstances.Add(newCard);
        }

        // Populate cards with inventory data
        List<InventoryCardData> dataForAllCards = GameManager.Inventory.GetDatas();
        PopulateGrids(ref dataForAllCards);

        // Prevent next executions
        _firstUpdateNotDone = false;
    }

    /// <summary>
    /// Instances a new InventoryCardObject in the scene with no data.
    /// </summary>
    /// <param name="parentInScene"> Parent to create card under. </param>
    /// <returns></returns>
    private GameObject InstanceNewCard(Transform parentInScene) {

        GameObject newCard = Instantiate(InventoryCardPrefab, parentInScene);

        // Setup data
        InventoryCardObject objectScript = newCard.GetComponent<InventoryCardObject>();
        objectScript.Parent = this;
        objectScript.SetCardToEmpty();

        // Return our new instance
        return newCard;
    }

    /// <summary>
    /// Each onEnable() checks if inventory content needs to be updated!
    /// If so populates again!
    /// </summary>
    private void CheckForUpdate() {

        bool outOfDateInformation = Mathf.Abs(_lastUpdateTime - GameManager.Inventory.inventoryLastUpdateTime) > 0.025f;

        if (!outOfDateInformation) {
            return;
        }

        // If out of dat info, do a new update!
        List<InventoryCardData> dataForAllCards = GameManager.Inventory.GetDatas();
        PopulateGrids(ref dataForAllCards);

        _lastUpdateTime = GameManager.Inventory.inventoryLastUpdateTime;
    }

    /// <summary>
    /// Clear grid data to repopulate with new data.
    /// </summary>
    private void ClearGrid() {
        foreach (GameObject card in _currentItemCardInstances) {
            card.GetComponent<InventoryCardObject>().SetCardToEmpty();
        }

        foreach (GameObject card in _currentInfoCardInstances) {
            card.GetComponent<InventoryCardObject>().SetCardToEmpty();
        }
    }

    /// <summary>
    /// Fill grid with our inventory data!
    /// </summary>
    /// <param name="dataForAllCards">Data to fill inventory grid with!</param>
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
