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

     [Header("Typewriter Settings")]
    public float typingSpeed = 0.05f; // Time between characters appearing
    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentSentence; // making sure the current sentence is types (for dialogue with only 1 sentence to correctly play the effect)

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
        inDialogue = true;
        
        dialogue.Clear();
        foreach (string currentString in sentences)
        {
            dialogue.Enqueue(currentString);
        }
        //foreach(string sentence in dialogue) Debug.Log($"{sentence}");
        DisplayNextSentence();
        //NextDialogue();
    }
    public void NextDialogue()
    {
        if (!inDialogue) return;
        //playerMovement.SetCanMove(false);

        // if currently typing characters, skip to the end of the text
        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentSentence;
                isTyping = false;
                return;
            }
        }
        else
        {
            // not typing, show the next sentence
            DisplayNextSentence();
        }
    }

    private void DisplayNextSentence()
    {
        if (dialogue.Count == 0) 
        {
            EndDialogue();
            return;
        }
        
        currentSentence = dialogue.Dequeue();
        Debug.Log($"Displaying: {currentSentence}");
        
        // Stop any existing typing coroutine
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        typingCoroutine = StartCoroutine(TypeText(currentSentence));
    }

    // Typewriter effect
    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        typingCoroutine = null;
    }

    public void EndDialogue()
    {
        Cursor.visible = false;
        dialogueBox.SetActive(false);
        inDialogue = false;
        playerMovement.SetCanMove(true);
    }
}