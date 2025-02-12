using UnityEngine;

public class RopeTrigger : MonoBehaviour
{
    public GameObject ropePrefab;  
    public Transform spawnPoint; 
    private GameObject ropeInstance;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Drone")) 
        {
            if (ropeInstance == null) 
            {
                Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
                ropeInstance = Instantiate(ropePrefab, spawnPosition, Quaternion.identity);
            }
        }
    }

}
