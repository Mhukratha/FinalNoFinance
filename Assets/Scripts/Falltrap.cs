using UnityEngine;

public class Falltrap : MonoBehaviour
{
   public Rigidbody2D trapRb; 
    public float fallDelay = 0.1f; 

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) 
        {
            Invoke("DropPlatform", fallDelay);
        }
    }

    void DropPlatform()
    {
        trapRb.bodyType = RigidbodyType2D.Dynamic;
    }
}
