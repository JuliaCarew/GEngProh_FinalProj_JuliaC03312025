using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ItemIcon
{
    public string itemName;
    public Sprite sprite;
}

public class UIManager : MonoBehaviour
{
    public GameManager gameManager;
    public QuestManager questManager;

    [Header("Controls")]
    public GameObject controlsDisplay;

    [Header("UI Screens")]
    public GameObject menuUI;
    public GameObject gameplayUI;
    public GameObject pausedUI;
    public GameObject optionsUI;

    [Header("Info Interactible UI")]
    public GameObject infoObj;
    public TextMeshProUGUI infoText;
    string infoString = "N - SunForge Mountain\r\nE - Village Graveyard\r\nS - Vibrant Meadow\r\nW - Crystal Cavern";

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
    public GameObject inventorySlotPrefab; // inventory item UI element

    public List<ItemIcon> itemIcons; // specific sprites for each item in inventory
    private Dictionary<string, Sprite> itemSpriteDict = new Dictionary<string, Sprite>();

    private void Awake()
    {
        foreach (var icon in itemIcons)
        {
            if (!itemSpriteDict.ContainsKey(icon.itemName))
                itemSpriteDict.Add(icon.itemName, icon.sprite);
        }
    }
    void Start()
    {
        if (questManager == null)
        {
            questManager = FindObjectOfType<QuestManager>();
        }
        // start all quest objects as inactive/blank
        questNotificationPanel.SetActive(false);
        controlsDisplay.SetActive(false);
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
        controlsDisplay.SetActive(true);
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

    // ***** INTERACTIBLES ***** //
    public IEnumerator DisplayInfoText()
    {
        Debug.Log("Started DisplayInfoText Coroutine");

        infoObj.SetActive(true);
        infoText.text = infoString;

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

    // Hide the quest notification panel after a delay
    private IEnumerator HideQuestNotificationAfterDelay()
    {
        yield return new WaitForSeconds(5);
        questNotificationPanel.SetActive(false);
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

    // use GetInventoryItemList() to get item list from inventory, then use this method to display item icons in UI
    public void ShowInventory()
    {
        if (inventorySlotPrefab == null || inventoryPanel == null)
        {
            Debug.LogError("UIManager: inventorySlotPrefab or inventoryPanel is not assigned!");
            return;
        }
        
        inventoryPanel.SetActive(true);
        
        foreach (string item in PlayerInventory.Instance.GetInventoryItemList())
        {
            GameObject newSlot = Instantiate(inventorySlotPrefab, inventoryPanel.transform);
            Image image = newSlot.GetComponent<Image>();
        }
    }
}