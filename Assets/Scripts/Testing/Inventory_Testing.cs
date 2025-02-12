using UnityEngine;

public class Inventory_Testing : MonoBehaviour
{
    public GameObject InventoryObject;
    private Inventory _inventory;

    // Start is called before the first frame update
    void Start()
    {
        _inventory = InventoryObject.GetComponent<Inventory>();
    }

    public void Print()
    {
        _inventory.Print();
    }

    public void SortTypeAscending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        _inventory.Print();
        _inventory.Sort(Inventory.SortParameters.TYPE, Inventory.SortOrder.ASCENDING);
        Debug.Log("After Sorting:");
        _inventory.Print();
    }

    public void SortTypeDescending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        _inventory.Print();
        _inventory.Sort(Inventory.SortParameters.TYPE, Inventory.SortOrder.DESCENDING);
        Debug.Log("After Sorting:");
        _inventory.Print();
    }

    public void SortNameAscending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        _inventory.Print();
        _inventory.Sort(Inventory.SortParameters.NAME, Inventory.SortOrder.ASCENDING);
        Debug.Log("After Sorting:");
        _inventory.Print();
    }

    public void SortNameDescending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        _inventory.Print();
        _inventory.Sort(Inventory.SortParameters.NAME, Inventory.SortOrder.DESCENDING);
        Debug.Log("After Sorting:");
        _inventory.Print();
    }

    public void SortIDAscending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        _inventory.Print();
        _inventory.Sort(Inventory.SortParameters.ID, Inventory.SortOrder.ASCENDING);
        Debug.Log("After Sorting:");
        _inventory.Print();
    }

    public void SortIDDescending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        _inventory.Print();
        _inventory.Sort(Inventory.SortParameters.ID, Inventory.SortOrder.DESCENDING);
        Debug.Log("After Sorting:");
        _inventory.Print();
    }
}
