using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public List<GameObject> inventory = new List<GameObject>();
    
    public void AddItemToInventory(string item){ // gets item from string, adds it to list 
        GameObject newItem = new GameObject(item);
        inventory.Add(newItem);
        Debug.Log($"Added {item} to inventory. Total items: {inventory.Count}");
    }
    public bool CheckInventoryForItem(string item){ // when checking for unique dialogue, get item name from string
        foreach(GameObject obj in inventory){
            if(obj.name == item){
                return true;
            }
        }
        return false;
    }
}
