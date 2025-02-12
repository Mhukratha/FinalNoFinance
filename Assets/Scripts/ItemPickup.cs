using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item item; // กำหนดไอเทมใน Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าผู้เล่นชนไอเทมหรือไม่
        {
            Inventory.instance.ShowItem(item);
            Destroy(gameObject); // ทำลายไอเทมหลังเก็บ
        }
    }
}