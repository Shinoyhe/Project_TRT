using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory_Testing : MonoBehaviour
{
    public GameObject inventoryObject;
    public Inventory inventory;

    // Start is called before the first frame update
    void Start()
    {
        inventory = inventoryObject.GetComponent<Inventory>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SortTypeAscending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        inventory.Print();
        inventory.Sort(Inventory.SortParameters.TYPE, Inventory.SortOrder.ASCENDING);
        Debug.Log("After Sorting:");
        inventory.Print();
    }

    public void SortTypeDescending()
    {
        Debug.Log("----------------------\nBefore Sorting:");
        inventory.Print();
        inventory.Sort(Inventory.SortParameters.TYPE, Inventory.SortOrder.DESCENDING);
        Debug.Log("After Sorting:");
        inventory.Print();
    }

    public void SortNameAscending()
    {
        inventory.Sort(Inventory.SortParameters.NAME, Inventory.SortOrder.ASCENDING);
    }
}
