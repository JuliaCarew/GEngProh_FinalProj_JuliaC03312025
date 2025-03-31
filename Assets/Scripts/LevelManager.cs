using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private GameManager _gameManager;
    private string spawnPointName;

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

         if (_gameManager == null)
        {
            _gameManager = GameManager.Instance; // assign GameManager singleton 
        }

        if (_gameManager.uiManager == null)
        {
            _gameManager.uiManager = FindObjectOfType<UIManager>();
        }
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