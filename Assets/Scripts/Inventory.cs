using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public static Inventory instance; // Singleton สำหรับเรียกใช้งานง่าย
    public Image itemDisplayUI; // UI Image สำหรับแสดงไอเทมที่เก็บได้
    public float displayTime = 50f; // เวลาที่ไอเทมจะโชว์บนหน้าจอ

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        if (itemDisplayUI != null)
            itemDisplayUI.gameObject.SetActive(false); // ซ่อน UI ตอนเริ่มเกม
    }

    public void ShowItem(Item item)
    {
        if (itemDisplayUI != null)
        {
            itemDisplayUI.sprite = item.icon;
            itemDisplayUI.gameObject.SetActive(true);
            CancelInvoke("HideItem");
            Invoke("HideItem", displayTime);
        }
    }

    private void HideItem()
    {
        itemDisplayUI.gameObject.SetActive(false);
    }
}