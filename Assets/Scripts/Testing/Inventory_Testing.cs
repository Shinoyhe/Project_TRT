using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Testing : MonoBehaviour
{
    public GameObject InventoryObject;
    private Inventory Inventory;

    // Start is called before the first frame update
    void Start()
    {
        Inventory = InventoryObject.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Print()
    {
        Inventory.Print();
    }

    public void SortTypeAscending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        Inventory.Print();
        Inventory.Sort(Inventory.SortParameters.TYPE, Inventory.SortOrder.ASCENDING);
        Debug.Log("After Sorting:");
        Inventory.Print();
    }

    public void SortTypeDescending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        Inventory.Print();
        Inventory.Sort(Inventory.SortParameters.TYPE, Inventory.SortOrder.DESCENDING);
        Debug.Log("After Sorting:");
        Inventory.Print();
    }

    public void SortNameAscending()
    {
        Inventory.Sort(Inventory.SortParameters.NAME, Inventory.SortOrder.ASCENDING);
    }
}
