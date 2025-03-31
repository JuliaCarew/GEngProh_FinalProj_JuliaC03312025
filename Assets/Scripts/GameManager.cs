using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public LevelManager levelManager;
    public PlayerMovement playerMovement;
    public GameStateManager gameStateManager;
    public UIManager uiManager;
    public DialogueManager dialogueManager;
    public Interactible_Controller interactibleController;
    public PlayerInventory playerInventory;
    public QuestManager questManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Ensure GameManager persists across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate GameManager instances
        }
    }

    private void Start()
    {
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }
    }
}
