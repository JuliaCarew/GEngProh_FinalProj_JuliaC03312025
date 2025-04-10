using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunForge : MonoBehaviour
{
    string theLegendarySword = "theLegendarySword"; // name of the sword item
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("SunForge: player collided. checking for sword crafting items.");
            CheckForSwordCrafting();
        }
    }
    // if the player has all 4 items, spawn the sword in the forge
    // and display win screen DisplayWinScreenUI()
    public void CheckForSwordCrafting()
    {
        if (PlayerInventory.Instance.CheckInventoryForItem("rubyPommel") &&
            PlayerInventory.Instance.CheckInventoryForItem("goldenwingedcowfeather") &&
            PlayerInventory.Instance.CheckInventoryForItem("banishedKnightsHelm") &&
            PlayerInventory.Instance.CheckInventoryForItem("osmiumBlade"))
        {
            PlayerInventory.Instance.RemoveItemFromInventory("rubyPommel");
            PlayerInventory.Instance.RemoveItemFromInventory("goldenwingedcowfeather");
            PlayerInventory.Instance.RemoveItemFromInventory("banishedKnightsHelm");
            PlayerInventory.Instance.RemoveItemFromInventory("osmiumBlade");

            //PlayerInventory.Instance.AddItemToInventory(theLegendarySword);
            Debug.Log("Sword materials cheched and removed.");
            // Display win screen UI
            GameManager.Instance.uiManager.DisplayWinScreenUI();
        }
    }
}


