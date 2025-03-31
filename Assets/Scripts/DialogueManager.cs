using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public GameManager gameManager;
    private PlayerMovement playerMovement;

    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    
    private Queue<string> dialogue;
    public bool inDialogue;

    void Awake()
    {
        dialogue = new Queue<string>();
        dialogueBox.SetActive(false);
    }
    void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameManager>();
        }
        if (gameManager != null)
        {
            playerMovement = gameManager.playerMovement;
        }
        else
        {
            playerMovement = FindObjectOfType<PlayerMovement>();
        }
    }

    public void StartDialogue(string[] sentences)
    {
        playerMovement.SetCanMove(false);
        Cursor.visible = true;
        dialogueBox.SetActive(true);
        
        dialogue.Clear();
        foreach (string currentString in sentences)
        {
            dialogue.Enqueue(currentString);
        }
        //foreach(string sentence in dialogue) Debug.Log($"{sentence}");

        NextDialogue();
    }
    public void NextDialogue()
    {
        inDialogue = true;
        playerMovement.SetCanMove(false);

        if (dialogue.Count == 0) EndDialogue();
        
        else if (dialogue.Count != 0)
        {
            Debug.Log($"{dialogue.Peek()}");
            dialogueText.text = dialogue.Peek();
            dialogue.Dequeue();
        }
    }
    public void EndDialogue()
    {
        Cursor.visible = false;
        dialogueBox.SetActive(false);
        inDialogue = false;
        playerMovement.SetCanMove(true);
    }
}