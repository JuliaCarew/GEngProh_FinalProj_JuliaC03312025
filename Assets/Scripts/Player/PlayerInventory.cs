using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    //public List<GameObject> inventory = new List<GameObject>();
    public static PlayerInventory Instance;
    public List<string> inventory = new List<string>();
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Keep inventory persistent across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItemToInventory(string item){ // gets item from string, adds it to list 
        inventory.Add(item);
        Debug.Log($"Added {item} to inventory. Total items: {inventory.Count}");
    }
    public bool CheckInventoryForItem(string item){ // when checking for unique dialogue, get item name from string
        return inventory.Contains(item);
    }
    public void RemoveItemFromInventory(string item){ // remove item from inventory

        if (inventory.Remove(item)) // Remove by name
        {
            Debug.Log($"Removed {item} from inventory. Total items: {inventory.Count}");
        }
        else
        {
            Debug.Log($"Item {item} not found in inventory.");
        }
    }
    public void ClearInventory(){ // clear inventory
        inventory.Clear();
        Debug.Log("Inventory cleared.");
    }
    public List<string> GetInventoryItemList(){
        return new List<string>(inventory);
    }
}
