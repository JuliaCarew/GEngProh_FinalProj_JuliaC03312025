using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quests : MonoBehaviour
{
    public QuestManager questManager;

    void Start()
    {
        // Create a quest
        Quest collectShrubberyQuest = questManager.CreateQuest(
            "Collect Shrubbery", 
            "Find the magical shrubbery for the knight"
        );

        // Add objectives
        questManager.AddQuestObjective(
            collectShrubberyQuest, 
            QuestObjective.ObjectiveType.Collect, 
            "Find the first shrubbery", 
            "shrubbery1"
        );
        questManager.AddQuestObjective(
            collectShrubberyQuest, 
            QuestObjective.ObjectiveType.Collect, 
            "Find the second shrubbery", 
            "shrubbery2"
        );
    }

    void OnTriggerEnter(Collider other)
    {
        // Start the quest
        questManager.StartQuest("Collect Shrubbery");
    }
}
