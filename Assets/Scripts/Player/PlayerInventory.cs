using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private UIManager uiManager; // Reference to UIManager for updating inventory UI  
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
    private void Start()
    {
        uiManager = FindObjectOfType<UIManager>();
        if (uiManager == null)
        {
            Debug.LogError("UIManager not found in the scene. Make sure it is present.");
        }
    }

    public void AddItemToInventory(string item){ // gets item from string, adds it to list 
        inventory.Add(item);
        Debug.Log($"Added {item} to inventory. Total items: {inventory.Count}");
        uiManager.UpdateInventoryUI(inventory);
    }
    public bool CheckInventoryForItem(string item){ // when checking for unique dialogue, get item name from string
        return inventory.Contains(item);
    }
    public void RemoveItemFromInventory(string item){ // remove item from inventory

        if (inventory.Remove(item)) // Remove by name
        {
            Debug.Log($"Removed {item} from inventory. Total items: {inventory.Count}");
            uiManager.UpdateInventoryUI(inventory);
        }
    }
    public void ClearInventory(){ // clear inventory
        inventory.Clear();
        Debug.Log("Inventory cleared.");
    }
    public List<string> GetInventoryItemList(){
        return new List<string>(inventory);
    }
    // have method to get item icons from inventory (make this UI Manager?) UI manager needs to display item icons in ShowInventory method
}
