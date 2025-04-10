using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Interactible_Controller.DialogueCondition;

public class Interactible_Controller : MonoBehaviour
{
    public GameManager gameManager;
    private DialogueManager dialogueManager;
    private QuestManager questManager;

    public bool isQuestItem;
    public bool isQuestNPC;
    public string NPCName; // will be assigned in Dialogue method
    public string questToStart; // Quest to start when interacting with this NPC
    public string questToComplete; // Quest to check for completion when interacting
    public string questToCheck; // Quest to check progress for specific dialogue

    // IMPORTANT: when adding new item, make sure to name it in all LOWER CASE, otherwise it will not be found in the inventory
    [System.Serializable] // allows for the creation of a new class in the inspector
    public class DialogueCondition{
        public enum ConditionType
        {
            Default,
            ItemCollected,
            AreaExplored,
            MonsterKilled,
            NPCTalkedTo,
            QuestActive,
            QuestCompleted,
            QuestObjectiveCompleted
        }
        public ConditionType conditionType;
        public string requiredConditionName; // name of the item required to trigger the dialogue event ID
        public string secondaryConditionName; // For multiple conditions (like deliver item to NPC)
        [TextArea]
        public string[] conditionalDialogue; // dialogue that is triggered if the player has the item
    }

    [TextArea]
    public string[] defaultDialogue;
    public List<DialogueCondition> dialogueConditions = new List<DialogueCondition>(); // list of all dialogue conditions( useful if this item has miltiple dialogue options)

    public InteractibleType interactibleType;
    public enum InteractibleType // sets the type of interaction for the object, swap between them in the inspector drop-down
    {
        Default,
        pickUp,
        Info,
        Dialogue
    }

    void Start()
    {
        StartCoroutine(InitializeAfterDelay());
    }
    private IEnumerator InitializeAfterDelay()
    {
        yield return new WaitForSeconds(0.1f); 

        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("interactibleController: GameManager instance is NULL!");
            yield break;
        }

        dialogueManager = gameManager.dialogueManager;
        questManager = gameManager.questManager;

        if (questManager == null)
        {
            Debug.LogError("interactibleController: QuestManager is NULL!");
        }
        else
        {
            Debug.Log("interactibleController: QuestManager successfully assigned.");
        }
    }
    
    public void Interact(){
        switch (interactibleType)
        {
            case InteractibleType.Default:
                Default();
                break;
            case InteractibleType.pickUp:
                PickUp();
                break;
            case InteractibleType.Info:
                Info();
                break;
            case InteractibleType.Dialogue:
                Dialogue(); 
                break;                       
        }
        // Check for quest-related interactions after checking everything else
        ProcessQuestInteraction();
    }
    private void Default()
    {
        Debug.Log($"InterController: Interacting with default {gameObject.name}");
    }
    private void PickUp() // add item to player Inventory
    {
        StartCoroutine(gameManager.uiManager.DisplayPickUpText("picked up " + gameObject.name));
        Debug.Log($"InterController: Picking up {gameObject.name}");
        
        // INVENTORY: Generic pickup to add any item to inventory
        gameManager.playerInventory.AddItemToInventory(gameObject.name.ToLower());
        Debug.Log($"InterController: {gameObject.name} added to inventory");

        // QUEST: Update quest objective when quest item is picked up
        if (isQuestItem)
        {
            questManager.CheckObjectiveCompletion(
            QuestObjective.ObjectiveType.Collect,
            gameObject.name.ToLower()
            );
        }

        this.gameObject.SetActive(false); // object disappears
    }

    // Info next to player's head, for inner thoughts in info on objects
    private void Info()
    {
        Debug.Log($"InterController: displaying info text");
        StartCoroutine(gameManager.uiManager.DisplayInfoText(defaultDialogue[0]));
    }

    private void Dialogue()  
    {
        // Assign NPC name if not already set
        if (string.IsNullOrEmpty(NPCName))
        {
            NPCName = gameObject.name;
            Debug.Log($"InterController: NPC name set to {NPCName}");
        }

        string[] dialogueToShow = defaultDialogue;
        bool conditionMet = false;

        foreach (var condition in dialogueConditions) // loop through all the conditions
        {
            if (CheckCondition(condition))
            {
                dialogueToShow = condition.conditionalDialogue;
                conditionMet = true;
                break;
            }
        }
        // DIALOGUE: If no conditions are met, start default dialogue
        dialogueManager.StartDialogue(dialogueToShow);

        // If it's an NPC talk objective and a condition was met
        if (isQuestNPC)
        {
            questManager.CheckObjectiveCompletion(
                QuestObjective.ObjectiveType.TalkTo,
                NPCName.ToLower()
            );
            Debug.Log($"Interacted with NPC: {NPCName}. Checking if TalkTo objective is completed.");
        }
    }

    private bool CheckCondition(DialogueCondition condition)
    {
        switch (condition.conditionType)
        {
            case DialogueCondition.ConditionType.ItemCollected: // take out dialoguecondition? not working if MULTIPLE conidional dialogue is this
                Debug.Log("checking dialogue condition type: Item Collected");
                return gameManager.playerInventory.CheckInventoryForItem(condition.requiredConditionName);
                
            case DialogueCondition.ConditionType.AreaExplored:
                return SceneManager.GetActiveScene().name.Equals(
                    condition.requiredConditionName, 
                    System.StringComparison.OrdinalIgnoreCase
                );
                
            case DialogueCondition.ConditionType.MonsterKilled:
                // need to track monster kills in GameManager
                return false;
                
            case DialogueCondition.ConditionType.NPCTalkedTo:
                // need to track NPC interactions !!! check if NPCName is requiredItemOrnpcName
                return questManager.HasTalkToObjective(NPCName);
                
            case DialogueCondition.ConditionType.QuestActive:
                return questManager.IsQuestActive(condition.requiredConditionName);
                
            case DialogueCondition.ConditionType.QuestCompleted:
                return questManager.IsQuestCompleted(condition.requiredConditionName);
                
            case DialogueCondition.ConditionType.QuestObjectiveCompleted:
                // need additional methods in QuestManager to check specific objectives
                return false;
                
            default:
                return false;
        }
    }

    private void ProcessQuestInteraction()
    {
        // If this is a Quest NPC that starts a quest
        if (isQuestNPC && !string.IsNullOrEmpty(questToStart))
        {
            // Only start the quest if it's not already active or completed
            if (!questManager.IsQuestActive(questToStart) && !questManager.IsQuestCompleted(questToStart))
            {
                questManager.StartQuest(questToStart);
            }
        }

        // If this is a Quest NPC that completes a quest
        if (isQuestNPC)
        {
            // check active quests for any delivery objectives matching this NPC
            foreach (var quest in questManager.activeQuests)
            {
                foreach (var objective in quest.questObjectives)
                {
                    if (objective.objectiveType == QuestObjective.ObjectiveType.Deliver && 
                    !objective.isCompleted)
                    {
                        // Check if this NPC is the delivery target
                        if (NPCName.ToLower() == objective.deliverToNpcName.ToLower())
                        {
                            // Check if player has the required item
                            if (gameManager.playerInventory.CheckInventoryForItem(objective.requiredItemOrnpcName))
                            {
                                Debug.Log($"Processing delivery of {objective.requiredItemOrnpcName} to {NPCName}");
                            
                                // Process the delivery
                                questManager.CheckObjectiveCompletion(
                                    QuestObjective.ObjectiveType.Deliver,
                                    objective.requiredItemOrnpcName,
                                    NPCName.ToLower()
                                );
                            }
                            else
                            {
                                Debug.Log($"Player does not have required item {objective.requiredItemOrnpcName} for delivery to {NPCName}");
                            }
                        }
                    }
                }
            }
        }
    }

    // update exploration objectives when entering a new scene
    public void CheckExplorationObjectives()
    {
        if (questManager == null)
        {
            Debug.LogWarning("⚠️ CheckExplorationObjectives called before QuestManager was assigned. Retrying...");
            StartCoroutine(DelayedCheckObjectives());
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;
        questManager.CheckObjectiveCompletion( // getting null error on scene loading
            QuestObjective.ObjectiveType.Explore,
            currentScene.ToLower()
        );
    }
    private IEnumerator DelayedCheckObjectives()
    {
        yield return new WaitForSeconds(0.1f); // give time for initialization
        CheckExplorationObjectives(); 
    }
}
