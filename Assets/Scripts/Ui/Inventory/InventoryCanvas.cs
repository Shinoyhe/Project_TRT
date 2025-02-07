using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private GameObject _inventoryCardPrefab;

    private List<InventoryCard> _inventoryCardScriptableObjects;
    private List<GameObject> _inventoryCardGameObjects = new List<GameObject>();

    private void OnEnable()
    {
        GameManager.Inventory.OnInventoryUpdated -= UpdateUI;
        GameManager.Inventory.OnInventoryUpdated += UpdateUI;
        _inventoryCardScriptableObjects = GameManager.Inventory.Get();

        UpdateUI();
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
        _inventoryCardScriptableObjects = GameManager.Inventory.Get();
    }

    private void UpdateUI()
    {
        _inventoryCardScriptableObjects = GameManager.Inventory.Get();
        UpdateCardGameObjects();
    }

    private void UpdateCardGameObjects()
    {
        foreach (GameObject cardObject in _inventoryCardGameObjects) {
            if (cardObject != null) {
                Destroy(cardObject);
            }
        }
        _inventoryCardGameObjects.Clear();

        foreach (InventoryCard card in _inventoryCardScriptableObjects) {
            
            GameObject newCard = Instantiate(_inventoryCardPrefab, _grid.transform);
            newCard.GetComponent<InventoryCardObject>().SetData(card);
            _inventoryCardGameObjects.Add(newCard);

            foreach (GameObject oldcard in _inventoryCardGameObjects) {
                print(oldcard.GetComponent<InventoryCardObject>().CardName);
            }
        }
    }
}
