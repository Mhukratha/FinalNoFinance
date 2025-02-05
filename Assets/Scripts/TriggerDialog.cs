using UnityEngine;

public class TriggerDialog : MonoBehaviour
{
    public TypewriterEffect typewriterEffect; // ระบบข้อความ
    public string[] dialogueLines; // ข้อความที่จะแสดง
    public GameObject dialogUI; // UI ที่ใช้แสดงข้อความ

    private bool hasTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTriggered) // ตรวจสอบว่าเป็น Player และยังไม่ Trigger มาก่อน
        {
            hasTriggered = true;
            dialogUI.SetActive(true); // แสดง UI
            typewriterEffect.StartDialogue(dialogueLines);
        }
    }
}
