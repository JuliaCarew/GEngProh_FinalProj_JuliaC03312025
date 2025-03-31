using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCollider : MonoBehaviour
{
    [SerializeField] public string sceneToLoad; // scene name to load
    [SerializeField] public string spawnPointName; // spawn point name

    private void OnTriggerEnter2D(Collider2D other) 
    {
        //Debug.Log("Trigger entered by: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log($"Player detected. Loading Scene: {sceneToLoad}, SpawnPoint: {spawnPointName}");
            
            LevelManager levelManager = FindObjectOfType<LevelManager>();
            if (levelManager != null)
            {
                levelManager.LoadSceneWithSpawnPoint(sceneToLoad, spawnPointName);
            }
        }
    }
}
