using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Creates and Inits InventoryCardObjects in grid format
/// Runs OnIventoryItemClick when cardObject is selected
/// </summary>
public class InventoryGridController : MonoBehaviour
{
    #region ======== [ PUBLIC PROPERTIES ] ========

    [Header("Settings")]
    public GameEnums.CardTypes TypeToDisplay;
    public bool UseSmallSize;
    public int InventorySize = 10;
    public bool SetDefaultSelectionOnEnable = false;

    [Header("Click Action")]
    public InventoryAction OnInventoryItemClick = null;

    #endregion

    #region ======== [ OBJECT REFERENCES ] ========

    [Header("Dependencies")]
    public GridLayoutGroup Grid;
    public GameObject InventoryCardPrefab;
    public AutoScrollGrid InventoryUiAutoScroller;

    #endregion

    #region ======== [ PRIVATE PROPERTIES ] ========

    private List<InventoryCardObject> _inventoryInstances = new List<InventoryCardObject>();

    private float _lastUpdateTime = 0f;
    private bool _createdInventory = false;

    #endregion

    #region ======== [ INIT METHODS ] ========
    private void OnEnable() {

        if (_createdInventory == false) {
            StartCoroutine("DelayInit");
        }

        if (_createdInventory && IsUpdateNeeded()) {
            PopulateGrid();
        }

        // Set default selection
        if(SetDefaultSelectionOnEnable && _inventoryInstances.Count > 0) {
            _inventoryInstances[0].CurrentActiveButton.Select();
        }
    }

    private void OnDisable() {
        GameManager.Inventory.OnInventoryUpdated -= PopulateGrid;
    }

    #endregion

    #region ======== [ PRIVATE METHODS ] ========

    /// <summary>
    /// Init delayed so GameManager can Initalize
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayInit() {
        yield return new WaitForSeconds(0.01f);
        GameManager.Inventory.OnInventoryUpdated += PopulateGrid;
        _createdInventory = true;
        CreateInventory();

        // Set default selection
        if (SetDefaultSelectionOnEnable && _inventoryInstances.Count > 0) {
            _inventoryInstances[0].CurrentActiveButton.Select();
        }
    }

    /// <summary>
    /// Checks if inventory has out of date information.
    /// </summary>
    /// <returns>True if inventory needs an update.</returns>
    private bool IsUpdateNeeded() {

        float updateTimeDelta = Mathf.Abs(_lastUpdateTime - GameManager.Inventory.inventoryLastUpdateTime);

        return updateTimeDelta > 0.025f;
    }

    /// <summary>
    /// Creates blank cards to setup the inventory.
    /// </summary>
    private void CreateInventory() {

        // Fill inventory will blank cards
        for (int i = 0; i < InventorySize; i++) {

            InventoryCardObject newInventoryItem = NewInventoryItem();

            // Init data
            newInventoryItem.InitalizeToGrid(i, InventoryUiAutoScroller, OnInventoryItemClick, UseSmallSize);

            // Add to instance list
            _inventoryInstances.Add(newInventoryItem);
        }

        PopulateGrid();
    }

    /// <summary>
    /// Add new card prefab on grid.
    /// </summary>
    /// <returns> The cards matching script. </returns>
    private InventoryCardObject NewInventoryItem() {
        GameObject cardObject = Instantiate(InventoryCardPrefab, Grid.transform);
        InventoryCardObject cardForInventory = cardObject.GetComponent<InventoryCardObject>();
        return cardForInventory;
    }

    /// <summary>
    /// Display inventory data in UI grid.
    /// </summary>
    private void PopulateGrid() {

        // Clear current data
        foreach (InventoryCardObject x in _inventoryInstances) {
            x.SetCardToEmpty(UseSmallSize);
        }

        int indexTracker = 0;

        // Get Data
        List<InventoryCardData> dataForAllCards = GameManager.Inventory.GetDatas();

        foreach (InventoryCardData card in dataForAllCards) {

            if (card.Type != TypeToDisplay) continue;

            InventoryCardObject currentInventoryItem = _inventoryInstances[indexTracker];

            currentInventoryItem.SetData(card);

            indexTracker += 1;

        }

        // Mark update time
        _lastUpdateTime = GameManager.Inventory.inventoryLastUpdateTime;
    }

    #endregion

}
