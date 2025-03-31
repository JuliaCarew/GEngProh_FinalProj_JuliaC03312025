using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public GameObject currentInteractible = null;  

    private void Update(){
        if (Input.GetKeyDown(KeyCode.E) && currentInteractible != null)
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
            Debug.Log($"Out of range for interactibles");
            currentInteractible = null;
        }
    }
}
