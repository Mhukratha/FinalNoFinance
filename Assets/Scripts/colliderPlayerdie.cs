using UnityEngine;
using UnityEngine.UI;

public class colliderPlayerdie : MonoBehaviour
{

    private void Start()
    {
     
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player hit!"); 
            Destroy(collision.gameObject);
        }
    }
}
