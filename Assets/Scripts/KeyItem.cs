using UnityEngine;

public class KeyItem : MonoBehaviour
{
    public GameObject pickupText;
    private bool isPlayerNearby = false;
    [SerializeField] CoreManager coreManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าเป็น Player และยังไม่ Trigger มาก่อน
        {
            pickupText.SetActive(true); // แสดง UI
            isPlayerNearby = true; // Player อยู่ใกล้
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าเป็น Player และยังไม่ Trigger มาก่อน
        {
            pickupText.SetActive(false); // ปิด UI
            isPlayerNearby = false; // Player อยู่ไกล
        }
    }

    private void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) // เช็คว่ากดปุ่ม E
        {
            PickupItem();
        }
    }

    private void PickupItem()
    {
        pickupText.SetActive(false); // ปิด UI
        coreManager.GetKeyItem();
    }
}
