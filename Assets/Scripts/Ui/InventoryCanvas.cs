using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryCanvas : MonoBehaviour
{

    [SerializeField] private GridLayoutGroup _grid;
    [SerializeField] private GameObject _inventory;
    [SerializeField] private GameObject _inventoryCardPrefab;

    private List<InventoryCard> _inventoryCardScriptableObjects;
    private List<GameObject> _inventoryCardGameObjects = new List<GameObject>();

    private void OnEnable()
    {
        _inventory.GetComponent<Inventory>().OnInventoryUpdated += UpdateUI;
        _inventoryCardScriptableObjects = _inventory.GetComponent<Inventory>().Get();
    }
    private void OnDisable()
    {
        if (_inventory != null)
        {
            _inventory.GetComponent<Inventory>().OnInventoryUpdated -= UpdateUI;
        }
    }
    private void OnDestroy()
    {
        if (_inventory != null)
        {
            _inventory.GetComponent<Inventory>().OnInventoryUpdated -= UpdateUI;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        _inventoryCardScriptableObjects = _inventory.GetComponent<Inventory>().Get();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateUI()
    {
        _inventoryCardScriptableObjects = _inventory.GetComponent<Inventory>().Get();
        UpdateCardGameObjects();
    }

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

        foreach (InventoryCard card in _inventoryCardScriptableObjects)
        {
            GameObject newCard = Instantiate(_inventoryCardPrefab, _grid.transform);
            newCard.GetComponent<InventoryCardObject>().SetData(card);
            _inventoryCardGameObjects.Add(newCard);

            foreach (GameObject oldcard in _inventoryCardGameObjects)
            {
                print(oldcard.GetComponent<InventoryCardObject>().CardName);
            }
        }
    }
}
