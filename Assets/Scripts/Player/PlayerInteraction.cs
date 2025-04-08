using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject currentInteractible = null;  
    private UIManager uiManager;

    private void Start()
    {
        // Register to the input events
        InputActions.InteractEvent += OnInteract;
        InputActions.OpenInventoryEvent += OnOpenInventory;
        
        // Get UIManager reference
        if (GameManager.Instance != null)
        {
            uiManager = GameManager.Instance.uiManager;
        }
        else
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }
    
    private void OnDestroy()
    {
        // Unregister from events when this object is destroyed
        InputActions.InteractEvent -= OnInteract;
        InputActions.OpenInventoryEvent -= OnOpenInventory;
    }

    private void OnInteract()
    {
        if (currentInteractible != null)
        {
            currentInteractible.GetComponent<Interactible_Controller>().Interact();
        }
    }
    
    private void OnOpenInventory()
    {
        // Call the method to open inventory UI
        if (uiManager != null)
        {
            //uiManager.ShowInventory();
            //Debug.Log("Opening inventory panel");
        }
        else
        {
            //Debug.LogWarning("UIManager not found, can't open inventory");
        }
    }


    private void Update(){
        if (Input.GetKeyDown(KeyCode.F) && currentInteractible != null)
        {
            currentInteractible.GetComponent<Interactible_Controller>().Interact();
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.tag == "Interactible")
        {
            //Debug.Log($"Interacting with {other.gameObject.name}");
            currentInteractible = other.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Interactible")
        {
            //Debug.Log($"Out of range for interactibles");
            currentInteractible = null;
        }
    }
}
