using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    public GameObject uiPanel; // ลาก UI Panel มาตรงนี้ใน Inspector

    private void Start()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(false); // ปิด UI ตอนเริ่มเกม
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่า Player ชนหรือไม่
        {
            Debug.Log("📌 Player entered the trigger zone!");
            if (uiPanel != null)
            {
                uiPanel.SetActive(true); // แสดง UI
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่า Player ออกจากโซน
        {
            Debug.Log("❌ Player exited the trigger zone!");
            if (uiPanel != null)
            {
                uiPanel.SetActive(false); // ซ่อน UI
            }
        }
    }
}
