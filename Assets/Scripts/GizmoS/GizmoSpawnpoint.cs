using UnityEngine;
using System.Collections;

public class Gizmo_SpawnPoint : MonoBehaviour
{
    // This script adds a gizmo in the scene view for spawn points
    // This method is called by Unity to draw the gizmo
    private void OnDrawGizmos()
    {
        // Draw a sphere at the spawn point
        Gizmos.color = Color.cyan;   // You can change the color here
        Gizmos.DrawWireSphere(transform.position, 0.10f); // Draw a sphere at the spawn point location with radius 0.5
    }
}
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private string spawnPointName; 

    private void Awake()
    {
        gameObject.name = spawnPointName; 
    }
}