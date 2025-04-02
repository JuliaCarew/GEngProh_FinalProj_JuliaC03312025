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

    // IMPORTANT: when adding new item, make sure to name it in all LOWER CASE, otherwise it will not be found in the inventory
    // make this an ENUM so multiple things can trigger dialogue (like exploring an area for a quest)
    // need: item
    // need: explored area
    // need: monster killed
    // need: talked to NPC
    [System.Serializable] // allows for the creation of a new class in the inspector
    public class DialogueCondition{
        public enum ConditionType
        {
            Default,
            itemCollected,
            areaExplored,
            monsterKilled,
            NPCTalkedTo
        }
        public ConditionType conditionType;
        public static string requiredConditionName; // name of the item required to trigger the dialogue event ID
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
        gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();
        dialogueManager = gameManager?.dialogueManager ?? FindObjectOfType<DialogueManager>();
        questManager = gameManager?.questManager ?? FindObjectOfType<QuestManager>();
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
                Dialogue(ConditionType.areaExplored); // want to be able to change this to current condition type
                break;
        }
    }
    private void Default()
    {
        Debug.Log($"InterController: Interacting with default {gameObject.name}");
    }
    private void PickUp() // add item to player Inventory
    {
        StartCoroutine(gameManager.uiManager.DisplayPickUpText("picked up " + gameObject.name));
        Debug.Log($"InterController: Picking up {gameObject.name}");
        this.gameObject.SetActive(false); // object disappears

        // INVENTORY: Generic pickup to add any item to inventory
        gameManager.playerInventory.AddItemToInventory(gameObject.name.ToLower());
        Debug.Log($"InterController: {gameObject.name} added to inventory");

        // !! should first be checking if its a quest item
        // QUEST: Update quest objective when quest item is picked up
        if (isQuestItem)
        {
            questManager.UpdateQuestObjective(
            QuestObjective.ObjectiveType.Collect,
            gameObject.name
            );
        }
    }

    // Info next to player's head, for inner thoughts in info on objects
    void Info()
    {
        Debug.Log($"InterController: displaying info text");
        StartCoroutine(gameManager.uiManager.DisplayInfoText());
    }

    private void Dialogue(ConditionType conditionType) // need to specify unique logic for different dialogue conditions
    {
        switch (conditionType)
        {
            case ConditionType.Default:
                Debug.Log("Dialogue condition checking: default");
                break;
            case ConditionType.itemCollected:
                CheckItemCollected();
                break;
            case ConditionType.areaExplored:
                CheckAreaExplored();
                break;
            case ConditionType.monsterKilled:
                CheckMonsterKilled();
                break;
            case ConditionType.NPCTalkedTo:
                CheckNPCTalkedTo();
                break;
        }

        foreach (var condition in dialogueConditions) // loop through all the conditions
        {
            if (gameManager.playerInventory.CheckInventoryForItem(requiredConditionName)) // check if player has the item
            {
                // DIALOGUE: If item is found, start dialogue for this condition
                dialogueManager.StartDialogue(condition.conditionalDialogue);
                return;
            }
        }
        // DIALOGUE: If no conditions are met, start default dialogue
        dialogueManager.StartDialogue(defaultDialogue);
    }
    private void CheckItemCollected()
    {
        Debug.Log("Dialogue condition checking: item collected");
    }
    private void CheckAreaExplored()
    {
        Debug.Log("Dialogue condition checking: area explored");
        Scene scene = SceneManager.GetActiveScene();

        if (scene.name == requiredConditionName) // needs to reference the string put in here
        {
            questManager.UpdateQuestObjective(
            QuestObjective.ObjectiveType.Explore,
            gameObject.name
            );
        }
    }
    private void CheckMonsterKilled()
    {
        Debug.Log("Dialogue condition checking: monster killed");
    }
    private void CheckNPCTalkedTo()
    {
        Debug.Log("Dialogue condition checking: NPC talked to");
    }
}
