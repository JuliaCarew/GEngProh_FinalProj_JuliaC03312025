using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public QuestManager questManager;

    [Header("UI Screens")]
    public GameObject menuUI;
    public GameObject gameplayUI;
    public GameObject pausedUI;
    public GameObject optionsUI;
    public GameObject winScreenUI;

    [Header("Location Text")]
    public TextMeshProUGUI locationText;

    [Header("Info Interactible UI")]
    public GameObject infoObj;
    public TextMeshProUGUI infoText;

    [Header("PickUp Interactible UI")]
    public GameObject pickUpObj;
    public TextMeshProUGUI pickUpText;

    [Header("Quest UI")]
    public GameObject questNotificationPanel;
    public GameObject questCompletedPanel;
  
    public TextMeshProUGUI questTitleText;
    public TextMeshProUGUI questDescriptionText;
    public TextMeshProUGUI currentObjectiveText;
    public TextMeshProUGUI questCompletedTitleText;

    [Header("Quest Reward UI")]
    [SerializeField] private GameObject rewardPanel;
    [SerializeField] private TextMeshProUGUI rewardItemText;
    [SerializeField] private Image rewardItemIcon;

    [Header("Player Inventory UI")]
    public GameObject inventoryPanel;
    [SerializeField] private List<Image> itemSlots; // UI Image slots
    [SerializeField] private Sprite emptySprite,wood,rock,rubyPommel,goldenWingedCowFeather,banishedKnightsHelm,osmiumBlade,theLegendarySword, lily, piece, maria, walkingStick, stick,trap; 

    void Start()
    {
        if (questManager == null)
        {
            questManager = FindObjectOfType<QuestManager>();
        }
        // start all quest objects as inactive/blank
        questNotificationPanel.SetActive(false);
    }
    
    public void UpdateInventoryUI(List<string> inventory)
    {
        Debug.Log("UIManager: Updating invenotry UI.");
        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < inventory.Count)
            {
                itemSlots[i].sprite = GetItemSprite(inventory[i]);
                itemSlots[i].enabled = true; // Enable the slot image
                itemSlots[i].color = new Color(1f, 1f, 1f, 1f); // Full opacity
            }
            else
            {
                itemSlots[i].sprite = emptySprite;
                itemSlots[i].enabled = true; 
                itemSlots[i].color = new Color(1f, 1f, 1f, 0.5f); // Half opacity for empty slots
            }
        }
    }
    private Sprite GetItemSprite(string itemName)
    {
        Debug.Log($"Getting sprite for item: {itemName} (lowercase: {itemName.ToLower()})");
        switch (itemName.ToLower()) 
        {
            case "wood": return wood;
            case "rock": return rock;
            case "rubypommel": return rubyPommel;
            case "goldenwingedcowfeather": return goldenWingedCowFeather;
            case "banishedknightshelm": return banishedKnightsHelm; 
            case "osmiumblade": return osmiumBlade;
            case "thelegendarysword": return theLegendarySword;
            case "lily": return lily;
            case "piece": return piece;
            case "maria": return maria;
            case "walkingstick": return walkingStick;
            case "stick": return stick;
            case "trap": return trap;
            default: return emptySprite;
        }
    }

    public void ToggleQuestIndicator(GameObject target, bool show)
    {
        Transform indicator = target.transform.Find("QuestIndicator");
        if (indicator != null)
        {
            indicator.gameObject.SetActive(show);
        }
    }

    public void DisplayMainMenuUI()
    {
        ClearUI();        
        menuUI.SetActive(true);
    }
    public void DisplayGameplayUI()
    {
        ClearUI();
        gameplayUI.SetActive(true);
    }
    public void DisplayPausedUI()
    {
        ClearUI();
        pausedUI.SetActive(true);
    }

    public void DisplayOptionsUI()
    {
        ClearUI();
        optionsUI.SetActive(true);
    }
    public void ClearUI()
    {
        if (menuUI != null) menuUI.SetActive(false);
        if (gameplayUI != null) gameplayUI.SetActive(false);
        if (pausedUI != null) pausedUI.SetActive(false);
        if (optionsUI != null) optionsUI.SetActive(false);
    }

    public void UpdateLocationText(string locationName)
    {
        if (locationText != null)
        {
            locationText.text = locationName;
        }
    }
    // ***** INTERACTIBLES ***** //
    public IEnumerator DisplayInfoText(string text)
    {
        Debug.Log("Started DisplayInfoText Coroutine");

        infoObj.SetActive(true);
        infoText.text = text;

        yield return new WaitForSeconds(5);
        infoObj.SetActive(false);
    }
    public IEnumerator DisplayPickUpText(string text)
    {
        Debug.Log("Started DisplayPickUpText Coroutine");

        pickUpObj.SetActive(true);
        pickUpText.text = text;

        yield return new WaitForSeconds(5);
        pickUpObj.SetActive(false);
    }   

    // ***** QUESTS ***** //
    public void DisplayQuestStartNotification(Quest quest)
    {
        //StartCoroutine(ShowQuestStartNotification(quest));
        ShowQuestStartNotification(quest);
    }

    /// Show the quest start notification for a specific quest
    private void ShowQuestStartNotification(Quest quest)
    {
        questNotificationPanel.SetActive(true);
        questTitleText.text = quest.questName;
        questDescriptionText.text = quest.questDescription;

        if (quest.questObjectives.Count > 0)
        {
            currentObjectiveText.text = quest.questObjectives[0].objectiveDescription;
        }
    }

    // Update the quest objective UI when a quest is updated
    public void UpdateQuestObjectiveUI(Quest quest)
    {
        QuestObjective currentObjective = quest.questObjectives.Find(obj => !obj.isCompleted);

        if (currentObjective != null)
        {
            // show current objective
            questNotificationPanel.SetActive(true);
            questTitleText.text = quest.questName;
            currentObjectiveText.text = currentObjective.objectiveDescription;
        }
    }

    // Show the quest completed notification for a specific quest
    public void DisplayQuestCompletedNotification(Quest quest)
    {
        StartCoroutine(ShowQuestCompletedNotification(quest));
    }

    /// Show the quest completed notification for a specific quest
    private IEnumerator ShowQuestCompletedNotification(Quest quest)
    {
        questCompletedPanel.SetActive(true);
        questCompletedTitleText.text = quest.questName;

        yield return new WaitForSeconds(5);
        questCompletedPanel.SetActive(false);
    }

    public void ShowAquireReward(string rewardItem)
    {
        rewardPanel.SetActive(true);
        rewardItemText.text = $"Obtained: {rewardItem} "+ "for completing the quest!"; 
        // set the icon for the reward item
        // rewardItemIcon.sprite = questManager.GetRewardItemIcon(rewardItem); 
    }
    public void HideRewardPanel()
    {
        rewardPanel.SetActive(false);
    }

    public void HideQuestNotificationPanel()
    {
        questNotificationPanel.SetActive(false);
    }

    public void DisplayWinScreenUI()
    {
        //if (gameplayUI != null) gameplayUI.SetActive(false);
        winScreenUI.SetActive(true);
    }
}