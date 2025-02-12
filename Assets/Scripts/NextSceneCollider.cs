using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneCollider : MonoBehaviour
{
    public string nextSceneName; // Assign the scene name in the Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure your Player has this tag
        {
            SceneManager.LoadScene(nextSceneName);
            Debug.Log("Next Scene");
        }
    }
}
