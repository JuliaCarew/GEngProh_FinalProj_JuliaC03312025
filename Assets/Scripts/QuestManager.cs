using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class QuestObjective{ // class for quest objectives
    public enum ObjectiveType
    {
        Nothing, // no objective, safeguard for when no objective is set
        Collect, // collect a certain quest item
        TalkTo, // talk to a quest npc
        Explore, // explore a certain area
        Deliver, // bring a quest item to a quest NPC
    }
    public ObjectiveType objectiveType; // type of objective
    public string objectiveDescription;
    public string requiredItemOrnpcName; // name of the item or npc required to complete the objective
    public string deliverToNpcName; // name of the npc to deliver the item to (if applicable)
    public bool isCompleted = false; // is the objective completed? automatically no
    public string rewardItem; // what you get for completing the quest objective
}

[System.Serializable]
public class Quest{ // class for quests
    public string questName;
    [TextArea]
    public string questDescription; // description of the quest

    public List<QuestObjective> questObjectives = new List<QuestObjective>(); // list of all objectives for the quest

    public bool isStarted = false;
    public bool isCompleted = false;

    [TextArea]
    public string startDialogue; // dialogue that is triggered when the quest is started
    [TextArea]
    public string completeDialogue; 

    public Action onQuestStarted; // action that is triggered when the quest is started
    public Action onQuestCompleted; // action that is triggered when the quest is completed

    public string questRewardItem; // item you get for completing the quest
}

public class QuestManager : MonoBehaviour
{
    public GameManager gameManager;
    public DialogueManager dialogueManager;
    public UIManager uiManager;

    public List<Quest> quests = new List<Quest>(); // list of ALL quests in the game
    public List<Quest> activeQuests = new List<Quest>(); // list of all ACTIVE quests in the game
    public List<Quest> completedQuests = new List<Quest>(); // list of all COMPLETED quests in the game

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();
        dialogueManager = gameManager?.dialogueManager ?? FindObjectOfType<DialogueManager>();
    }

    public void StartQuest(string questName)
    {
        Quest questToStart = quests.Find(q => q.questName == questName); // find the quest in the list of quests

        if (questToStart != null && !questToStart.isStarted) 
        {
            questToStart.isStarted = true; // set the quest as started
            activeQuests.Add(questToStart); // add the quest to the list of active quests

            if (!string.IsNullOrEmpty(questToStart.startDialogue)) 
            {
                dialogueManager.StartDialogue(new string[] { questToStart.startDialogue }); // start the dialogue for the quest
            }

            uiManager.DisplayQuestStartNotification(questToStart); // display quest start UI notification

            questToStart.onQuestStarted?.Invoke(); 
            UpdateQuestIndicators();
            Debug.Log($"Quest Started: {questName}");
        }
    }
    
    // _______________________ Check for objective completion _______________________ //
    public void CheckObjectiveCompletion(QuestObjective.ObjectiveType objectiveType, string objectiveTarget, string secondaryTarget = "")
    {
        if (activeQuests == null)
        {
            Debug.LogError("QuestManager: activeQuests is NULL!");
            return;
        }

        if (activeQuests.Count == 0)
        {
            Debug.LogWarning("QuestManager: No active quests to check.");
            return;
        }

        foreach (Quest quest in activeQuests) // loop through all active quests (causing error on quest completed, collection was modified during iteration) 
        {
            if (quest == null)
            {
                Debug.LogError("QuestManager: A quest in activeQuests is NULL!");
                continue;
            }

            bool questCompleted = true; // assume the quest is completed

            foreach (var objective in quest.questObjectives) // loop through all objectives in the quest
            {
                if (objective == null)
                {
                    Debug.LogError($"QuestManager: A quest objective in {quest.questName} is NULL!");
                    continue;
                }

                if (objective.isCompleted) continue;

                if (objective.objectiveType == objectiveType)
                {
                    // For Deliver objectives, check if the item name matches
                    if (objectiveType == QuestObjective.ObjectiveType.Deliver)
                    {
                        Debug.Log($"Checking Deliver: Item={objectiveTarget}, NPC={secondaryTarget}, Required={objective.requiredItemOrnpcName}, DeliverTo={objective.deliverToNpcName}");
                    
                        // Check if this is the right delivery objective
                        if (objective.requiredItemOrnpcName.ToLower() == objectiveTarget.ToLower())
                        {
                            Deliver(objectiveTarget, secondaryTarget, objective);
                        }
                    }
                    else if (objective.requiredItemOrnpcName.ToLower() == objectiveTarget.ToLower())
                    {
                        switch (objectiveType)
                        {
                            case QuestObjective.ObjectiveType.Collect:
                                Collect(objectiveTarget, objective);
                                break;
                            case QuestObjective.ObjectiveType.TalkTo:
                                TalkTo(objectiveTarget, objective);
                                break;
                            case QuestObjective.ObjectiveType.Explore:
                                Explore(objectiveTarget, objective);
                                break;
                            default:
                                break;
                        }
                        Debug.Log($"Objective Completed: {objective.objectiveDescription}");
                    }
                }
                if (!objective.isCompleted){
                    questCompleted = false; 
                }
            }
            // Complete the quest if all objectives are done
            if (questCompleted && quest.questObjectives.Count > 0)
            {
                CompleteQuest(quest.questName);
            }
        }
    }

    // _______________________ Complete the quest _______________________ //
    public void CompleteQuest(string questName)
    {
        Quest questToComplete = activeQuests.Find(q => q.questName == questName); // find the quest in the list of active quests

        if (questToComplete != null && !questToComplete.isCompleted) // if the quest is found and not completed
        {
            questToComplete.isCompleted = true;
            activeQuests.Remove(questToComplete); // remove the quest from the list of active quests
            completedQuests.Add(questToComplete); // add the quest to the list of completed quests

            foreach (var objective in questToComplete.questObjectives)
            { // turn indicators off for all objectives in the quest
                GameObject target = GameObject.Find(objective.requiredItemOrnpcName);
                if (target != null)
                    uiManager.ToggleQuestIndicator(target, false);

                if (objective.objectiveType == QuestObjective.ObjectiveType.Deliver &&
                    !string.IsNullOrEmpty(objective.deliverToNpcName))
                {
                    GameObject deliveryNpc = GameObject.Find(objective.deliverToNpcName);
                    if (deliveryNpc != null)
                        uiManager.ToggleQuestIndicator(deliveryNpc, false);
                }
            }

            if (!string.IsNullOrEmpty(questToComplete.questRewardItem)) // give player reqard item for completing quest
            {
                RecieveRewardItem(questToComplete.questRewardItem);
            }
            if (!string.IsNullOrEmpty(questToComplete.completeDialogue)) // if the quest has a complete dialogue
            {
                //dialogueManager.StartDialogue(new string[] { questToComplete.completeDialogue }); // start the completed dialogue for the quest
                // displays NPCs conditional dialogue instead
            }
            
            uiManager.DisplayQuestCompletedNotification(questToComplete); 
            uiManager.HideQuestNotificationPanel(); 

            questToComplete.onQuestCompleted?.Invoke(); // invoke the action for when the quest is completed

            Debug.Log($"Quest Completed: {questName}");
        }
    }

    // _______________________ Create a new quest _______________________ //
    public Quest CreateQuest(string questName, string description, string rewardItem)
    {
        Quest newQuest = new Quest
        {
            questName = questName,
            questDescription = description,
            questRewardItem = rewardItem
        }; // create a new quest from class
        quests.Add(newQuest); // add the new quest to the list of quests
        return newQuest; 
    }

    // _______________________ Add a new objective to the quest _______________________ //
    public void AddQuestObjective(Quest quest, QuestObjective.ObjectiveType type, string objectiveDescription, string requiredTarget, string rewardItem)
    {
        QuestObjective newObjective = new QuestObjective
        {
            objectiveType = type,
            objectiveDescription = objectiveDescription,
            requiredItemOrnpcName = requiredTarget,
            rewardItem = rewardItem
        }; // create a new objective from class

        quest.questObjectives.Add(newObjective); // add the new objective to the quest
        Debug.Log($"Objective Added: {objectiveDescription} to quest {quest.questName}");
    }

    // Check list of Active quests
    public bool IsQuestActive(string questName)
    {
        return activeQuests.Exists(q => q.questName == questName); 
    }

    // Check list of Completed quests
    public bool IsQuestCompleted(string questName)
    {
        return completedQuests.Exists(q => q.questName == questName); 
    }

    // _______________________ Objective types _______________________ //
    private void Collect(string itemName, QuestObjective objective)
    {
        // check the player's inventory for the 'item to collect'
        // if found, update quest UI and mark objective as completed
        if (gameManager.playerInventory.CheckInventoryForItem(itemName))
        {
            CompleteObjective(objective);
        }
    }
    private void TalkTo(string npcName, QuestObjective objective)
    {
        // take a NPC string name and check if the player has interacted with them
        // if so, mark objective as complete
        if (objective.isCompleted) return;

        // Log for debugging
        Debug.Log($"Checking TalkTo objective: {objective.objectiveDescription} for NPC: {npcName} vs required: {objective.requiredItemOrnpcName}");

        // Check if the NPC name matches the required NPC for the quest objective
        if (npcName.Equals(objective.requiredItemOrnpcName, StringComparison.OrdinalIgnoreCase))
        {
            // Objective is completed when the correct NPC is interacted with
            CompleteObjective(objective);
            Debug.Log($"TalkTo objective completed for NPC: {npcName}");
        }
    }
    private void Explore(string areaName, QuestObjective objective)
    {
        // name each level in the scenes
        // if the scene's name = explore objective name, complete objective
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene.Equals(areaName, StringComparison.OrdinalIgnoreCase))
        {
            CompleteObjective(objective);
        }
    }
    private void Deliver(string itemToDeliver, string npcName, QuestObjective objective)
    {
        // check the player's inventory for the 'item to deliver'
        // if found, remove it from the inventory and update quest objective
        // when interacting with the specified deliverTO NPC, update quest objective
        if (objective.deliverToNpcName == null)
        {
            Debug.LogError($"Delivery objective for {itemToDeliver} has null deliverToNpcName!");
            return;
        }
        
        Debug.Log($"Checking delivery: Item={itemToDeliver}, NPC={npcName}, Target NPC={objective.deliverToNpcName}");

        if (gameManager.playerInventory.CheckInventoryForItem(itemToDeliver) && 
        npcName.ToLower() == objective.deliverToNpcName.ToLower())
        {
            // Remove the item from inventory
            gameManager.playerInventory.RemoveItemFromInventory(itemToDeliver);            

            // Complete the objective
            CompleteObjective(objective);
        
            Debug.Log($"Delivered {itemToDeliver} to {npcName}. Objective completed!");
        }
        else
        {
            Debug.Log($"Cannot complete delivery objective. Item in inventory: {gameManager.playerInventory.CheckInventoryForItem(itemToDeliver)}, Correct NPC: {npcName.Equals(objective.deliverToNpcName, StringComparison.OrdinalIgnoreCase)}");
        }
    }
    // _______________________ Objective logic end _______________________ //
    
    private void CompleteObjective(QuestObjective objective)
    {
        if (objective == null) return;
        
        objective.isCompleted = true;
        
        if (!string.IsNullOrEmpty(objective.rewardItem))
        {
            gameManager.playerInventory.AddItemToInventory(objective.rewardItem); // add the reward item to inventory(making some slots null)
        }
        
        // Find which quest this objective belongs to for UI update
        Quest parentQuest = activeQuests.Find(q => q.questObjectives.Contains(objective));
        if (parentQuest != null)
        {
            uiManager.UpdateQuestObjectiveUI(parentQuest);
        }
        UpdateQuestIndicators();
        Debug.Log($"Objective Completed: {objective.objectiveDescription}");
    }
    private void RecieveRewardItem(string rewardItem)
    {
        // take the string reward item and find the gameobject with that name?
        gameManager.playerInventory.AddItemToInventory(rewardItem);

        if (uiManager != null)
        {
            uiManager.ShowAquireReward(rewardItem); // Show the reward recieved UI
        }
        Debug.Log($"Received reward item: {rewardItem}");
    }


    public bool HasTalkToObjective(string npcName)
    {
        foreach (Quest quest in activeQuests)
        {
            foreach (QuestObjective objective in quest.questObjectives)
            {
                if (objective.objectiveType == QuestObjective.ObjectiveType.TalkTo && 
                objective.requiredItemOrnpcName.Equals(npcName, StringComparison.OrdinalIgnoreCase) &&
                !objective.isCompleted)
                {
                    return true; // found an active quest that requires talking to this NPC
                }
            }
        }
        return false; // no active TalkTo objective for this NPC
    }

    // _______________________ Quest indicators _______________________ //
    public void UpdateQuestIndicators()
    {
        foreach (Quest quest in activeQuests)
        {
            foreach (QuestObjective objective in quest.questObjectives)
            {
                if (objective.isCompleted) continue;

                // Get NPC or item GameObject by name
                GameObject target = GameObject.Find(objective.requiredItemOrnpcName);

                if (target != null)
                {
                    uiManager.ToggleQuestIndicator(target, true);
                }

                // If it's a delivery quest, also highlight the delivery target NPC
                if (objective.objectiveType == QuestObjective.ObjectiveType.Deliver &&
                    !string.IsNullOrEmpty(objective.deliverToNpcName))
                {
                    GameObject deliveryNpc = GameObject.Find(objective.deliverToNpcName);
                    if (deliveryNpc != null)
                    {
                        uiManager.ToggleQuestIndicator(deliveryNpc, true);
                    }
                }
            }
        }
    }
}
