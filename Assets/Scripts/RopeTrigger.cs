using UnityEngine;

public class RopeTrigger : MonoBehaviour
{
    public GameObject ropePrefab;  // Assign your rope prefab in the inspector
    private GameObject ropeInstance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("RopeTrigger")) // Ensure the trigger object has this tag
        {
            if (ropeInstance == null) // Prevent multiple ropes from spawning
            {
                ropeInstance = Instantiate(ropePrefab, transform.position, Quaternion.Euler(0, 0, 90));
            }
        }
    }

   
}
