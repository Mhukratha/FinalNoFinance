using UnityEngine;

public class OnAwakeDialog : MonoBehaviour
{

    public TypewriterEffect typewriterEffect; // ระบบข้อความ
    public string[] dialogueLines; // ข้อความที่จะแสดง
    public GameObject dialogUI; // UI ที่ใช้แสดงข้อความ
    public PlayerController playerController;

    void Awake() 
    {
        playerController.enabled = false;
        StartDialogues();
    }

    private void StartDialogues()
    {
        dialogUI.SetActive(true); // แสดง UI
        typewriterEffect.StartDialogue(dialogueLines);
    }
}
