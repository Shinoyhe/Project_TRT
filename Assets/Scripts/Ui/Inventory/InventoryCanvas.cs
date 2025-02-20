using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private GameObject _inventoryCardPrefab;

    private List<InventoryCardData> _inventoryCardScriptableObjects;
    private List<GameObject> _inventoryCardGameObjects = new List<GameObject>();

    private void OnEnable()
    {
        StartCoroutine("WaitForGameManager");
    }

    private void OnDisable()
    {
        if (GameManager.Inventory != null) {
            GameManager.Inventory.OnInventoryUpdated -= UpdateUI;
        }
    }

    private void OnDestroy()
    {
        if (GameManager.Inventory != null) {
            GameManager.Inventory.OnInventoryUpdated -= UpdateUI;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _inventoryCardScriptableObjects = GameManager.Inventory.GetDatas();
    }

    /// <summary>
    /// Updates all of the Card UI elements on the screen to ensure they have accurate data
    /// </summary>
    /// <returns></returns>
    private void UpdateCardGameObjects()
    {
        foreach (GameObject cardObject in _inventoryCardGameObjects)
        {
            if (cardObject != null)
            {
                Destroy(cardObject);
            }
        }
        _inventoryCardGameObjects.Clear();

        foreach (InventoryCardData card in _inventoryCardScriptableObjects)
        {

            GameObject newCard = Instantiate(_inventoryCardPrefab, _grid.transform);
            newCard.GetComponent<InventoryCardObject>().SetData(card);
            _inventoryCardGameObjects.Add(newCard);
        }
    }

    private void UpdateUI()
    {
        _inventoryCardScriptableObjects = GameManager.Inventory.GetDatas();
        UpdateCardGameObjects();
    }

    /// <summary>
    /// Once the GameManager object exists, sets up the inventory screen
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForGameManager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);

        GameManager.Inventory.OnInventoryUpdated -= UpdateUI;
        GameManager.Inventory.OnInventoryUpdated += UpdateUI;
        _inventoryCardScriptableObjects = GameManager.Inventory.GetDatas();

        UpdateUI();
    }
}
