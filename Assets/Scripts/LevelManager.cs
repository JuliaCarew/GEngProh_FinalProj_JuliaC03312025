using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private GameManager gameManager;
    private QuestManager questManager;
    private string spawnPointName;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }

    public void LoadSceneWithSpawnPoint(string sceneName, string spawnPoint)
    {
        spawnPointName = spawnPoint;
        SceneManager.sceneLoaded += OnSceneLoaded; // subscribe 
        SceneManager.LoadScene(sceneName); 
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; // unsubscribe
        SetPlayerToSpawn();

        if (gameManager == null)
        {
            gameManager = GameManager.Instance; // assign GameManager singleton 
        }

        if (gameManager.uiManager == null)
        {
            gameManager.uiManager = FindObjectOfType<UIManager>();
        }
        StartCoroutine(DelayedSceneLoadActions());
    }
    private IEnumerator DelayedSceneLoadActions()
    {
        yield return new WaitForSeconds(0.1f); // wait for GameManager to initialize
        gameManager.interactibleController?.CheckExplorationObjectives();
    }

    private void SetPlayerToSpawn()
    {
        GameObject spawnPoint = GameObject.Find(spawnPointName);
        if (spawnPoint != null) {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                player.transform.position = spawnPoint.transform.position;
                Debug.Log($"Player spawned at {spawnPointName}");
            }
        }
        else
        {
            Debug.LogWarning($"Spawn point '{spawnPointName}' not found in the scene!");
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

    // change scene from main menu to gameplay
    public void StartGame()
    {
        LoadSceneWithSpawnPoint("Gameplay", "SpawnPoint");
    }

    public void Quit()
    {
        Debug.Log("Quit");
        Application.Quit();
    }
}