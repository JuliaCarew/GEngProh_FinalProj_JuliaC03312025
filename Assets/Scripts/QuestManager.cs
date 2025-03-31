using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestObjective{
    public enum ObjectiveType
    {
        Nothing, // no objective, safeguard for when no objective is set
        Collect, // collect a certain item
        TalkTo, // talk to a certain npc, complete certain dialogues
        Explore, // explore a certain area
        Deliver, // AND Escort, same logic. bring item/person to another location
        Kill // killing npc or monster
    }
    public ObjectiveType objectiveType; // type of objective
    public string objectiveDescription;
    public string requiredItemOrnpcName; // name of the item or npc required to complete the objective
    public bool isCompleted = false; // is the objective completed? automatically no
}

[System.Serializable]
public class Quest{
    public string questName;
    [TextArea]
    public string questDescription; // description of the quest

    public List<QuestObjective> questObjectives = new List<QuestObjective>(); // list of all objectives for the quest

    public bool isStarted = false;
    public bool isCompleted = false;

    [TextArea]
    public string startDialogue; // dialogue that is triggered when the quest is started
    [TextArea]
    public string completeDialogue; // dialogue that is triggered when the quest is completed

    public Action onQuestStarted; // action that is triggered when the quest is started
    public Action onQuestCompleted; // action that is triggered when the quest is completed
}

public class QuestManager : MonoBehaviour
{
    public GameManager gameManager;
    public DialogueManager dialogueManager;
    public UIManager uiManager;

    public List<Quest> quests = new List<Quest>(); // list of ALL quests in the game
    public List<Quest> activeQuests = new List<Quest>(); // list of all ACTIVE quests in the game
    public List<Quest> completedQuests = new List<Quest>(); // list of all COMPLETED quests in the game

    void Start()
    {
        gameManager = GameManager.Instance ?? FindObjectOfType<GameManager>();
        dialogueManager = gameManager?.dialogueManager ?? FindObjectOfType<DialogueManager>();
    }

    public void StartQuest(string questName)
    {
        Quest questToStart = quests.Find(q => q.questName == questName); // find the quest in the list of quests

        if (questToStart != null && !questToStart.isStarted) // if the quest is found and not started
        {
            questToStart.isStarted = true; // set the quest as started
            activeQuests.Add(questToStart); // add the quest to the list of active quests

            if (!string.IsNullOrEmpty(questToStart.startDialogue)) // if the quest has a start dialogue
            {
                dialogueManager.StartDialogue(new string[] { questToStart.startDialogue }); // start the dialogue for the quest
            }

            uiManager.DisplayQuestStartNotification(questToStart); // UIMANAGER: Display quest start UI notification

            questToStart.onQuestStarted?.Invoke(); // invoke the action for when the quest is started

            Debug.Log($"Quest Started: {questName}");
        }
    }
    
    public void UpdateQuestObjective(QuestObjective.ObjectiveType objectiveType, string objectiveTarget)
    {
        foreach (Quest quest in activeQuests) // loop through all active quests
        {
            bool questCompleted = true; // assume the quest is completed

            foreach (QuestObjective objective in quest.questObjectives) // loop through all objectives in the quest
            {
                if (objective.objectiveType == objectiveType && objective.requiredItemOrnpcName.ToLower() == objectiveTarget) // if the objective type and target match
                {
                    objective.isCompleted = true; // set the objective as completed
                    
                    uiManager.UpdateQuestObjectiveUI(quest); // UIMANAGER: Update UI with new objective
                    
                    Debug.Log($"Objective Completed: {objective.objectiveDescription}");
                }
                if (!objective.isCompleted){
                    questCompleted = false; // if any objective is not completed, the quest is not completed
                }
            }
            // Complete the quest if all objectives are done
            if (questCompleted)
            {
                CompleteQuest(quest.questName);
            }
        }
    }

    public void CompleteQuest(string questName)
    {
        Quest questToComplete = activeQuests.Find(q => q.questName == questName); // find the quest in the list of active quests

        if (questToComplete != null && !questToComplete.isCompleted) // if the quest is found and not completed
        {
            questToComplete.isCompleted = true; // set the quest as completed
            activeQuests.Remove(questToComplete); // remove the quest from the list of active quests
            completedQuests.Add(questToComplete); // add the quest to the list of completed quests

            if (!string.IsNullOrEmpty(questToComplete.completeDialogue)) // if the quest has a complete dialogue
            {
                dialogueManager.StartDialogue(new string[] { questToComplete.completeDialogue }); // start the dialogue for the quest
            }
            
            uiManager.DisplayQuestCompletedNotification(questToComplete); // UIMANAGER: Display quest completed UI notification

            questToComplete.onQuestCompleted?.Invoke(); // invoke the action for when the quest is completed

            Debug.Log($"Quest Completed: {questName}");
        }
    }

    // Create a new quest
    public Quest CreateQuest(string questName, string description)
    {
        Quest newQuest = new Quest
        {
            questName = questName,
            questDescription = description,
            //isStarted = false,
            //isCompleted = false
        }; // create a new quest object
        quests.Add(newQuest); // add the new quest to the list of quests
        return newQuest; 
    }

    // Add a new objective to the quest
    public void AddQuestObjective(Quest quest, QuestObjective.ObjectiveType type, string objectiveDescription, string requiredTarget)
    {
        QuestObjective newObjective = new QuestObjective{
            objectiveType = type,
            objectiveDescription = objectiveDescription,
            requiredItemOrnpcName = requiredTarget,
            //isCompleted = false
        }; // create a new objective object

        quest.questObjectives.Add(newObjective); // add the new objective to the quest
        Debug.Log($"Objective Added: {objectiveDescription} to quest {quest.questName}");
    }

    // Check if a specific quest is active
    public bool IsQuestActive(string questName)
    {
        return activeQuests.Exists(q => q.questName == questName); // check if the quest is in the list of active quests
    }

    // Check if a specific quest is completed
    public bool IsQuestCompleted(string questName)
    {
        return completedQuests.Exists(q => q.questName == questName); // check if the quest is in the list of completed quests
    }
}
