
using UnityEngine;
using System.Collections;

public class Linghtningtrap : MonoBehaviour
{
     public GameObject firePrefab;  // ใช้ Prefab ของ Particle System แทน
    public float fireInterval = 3f; // ระยะเวลาปล่อยไฟทุกๆ กี่วินาที
    public float fireDuration = 1f; // ระยะเวลาที่ไฟจะเปิด
    public GameObject gameOverUI;   // UI Game Over

    private GameObject currentFire; // เก็บ Particle ที่ถูกสร้างขึ้น
    private bool isFiring = false;

    private void Start()
    {
        gameOverUI.SetActive(false); 
        InvokeRepeating(nameof(ShootFire), 0f, fireInterval);
    }

    private void ShootFire()
    {
        currentFire = Instantiate(firePrefab, transform.position, Quaternion.identity);
        //Debug.Log("⚡Activated!");
        isFiring = true;
        Destroy(currentFire, fireDuration);
        Invoke(nameof(StopFire), fireDuration);
   
    }

    private void StopFire()
    {
        isFiring = false;
        //Debug.Log("⚡Stopped.");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isFiring && collision.CompareTag("Player"))
        {
            Debug.Log("⚡ Player hit");
            gameOverUI.SetActive(true);
            Destroy(collision.gameObject); 
        }
    }
}

