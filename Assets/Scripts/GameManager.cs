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
            DontDestroyOnLoad(gameObject); // ✅ Keeps GameManager alive

            // Ensure QuestManager is assigned
            if (questManager == null)
            {
                questManager = FindObjectOfType<QuestManager>();
                if (questManager == null)
                {
                    Debug.LogError("❌ GameManager: QuestManager is missing in the scene!");
                }
                else
                {
                    Debug.Log("✅ GameManager: QuestManager assigned.");
                }
            }
        }
        else
        {
            Destroy(gameObject); // ✅ Prevent duplicate GameManagers
        }
    }

    private void Start()
    {
        AssignSceneReferences();
    }
    private void AssignSceneReferences()
    {
        interactibleController = FindObjectOfType<Interactible_Controller>();
        questManager = FindObjectOfType<QuestManager>();
        uiManager = FindObjectOfType<UIManager>();
        playerInventory = FindObjectOfType<PlayerInventory>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }

    private void OnEnable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        AssignSceneReferences();
    }
}
