using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TypewriterEffect : MonoBehaviour
{
    public Text dialogText; // UI Text สำหรับแสดงข้อความ
    public float typingSpeed = 0.05f; // ความเร็วในการพิมพ์
    private string[] lines; // เก็บข้อความทั้งหมด
    private int index = 0;
    private bool isTyping = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            NextLine();
        }
    }

    public void StartDialogue(string[] dialogueLines)
    {
        lines = dialogueLines;
        index = 0;
        dialogText.text = "";
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        dialogText.text = "";

        foreach (char letter in lines[index].ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    public void NextLine()
    {
        if (isTyping) return; // หยุดถ้าอยู่ระหว่างการพิมพ์

        if (index < lines.Length - 1)
        {
            index++;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false); // ซ่อน UI เมื่อข้อความจบ
        }
    }
}
