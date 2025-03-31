using UnityEngine;

public class Gizmo_TriggerSceneChange : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // Draw a wireframe cube matching the BoxCollider2D size
        Gizmos.color = new Color(0, 255, 255, 0.75f); // Set Gizmo color to cyan

        // Get the BoxCollider2D component
        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider != null)
        {
            // Calculate the center position and size of the WireCube
            Vector3 center = transform.position + (Vector3)boxCollider.offset;
            Vector3 size = new Vector3(boxCollider.size.x, boxCollider.size.y, 0);

            // Draw the WireCube
            Gizmos.DrawCube(center, size);
        }
    }
}
