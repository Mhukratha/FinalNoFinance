using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CoreManager : MonoBehaviour
{
    [SerializeField] private bool haveKeyItem = false;
    private bool isPlayerNearby = false;
    public GameObject keyItem;
    public GameObject dialogText;
    public GameObject giveKeyText;
    public GameObject damagedBot;
    public GameObject newBot;
    [SerializeField] private FadeIn fade;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าเป็น Player และยังไม่ Trigger มาก่อน
        {
            if (haveKeyItem)
            {
                giveKeyText.SetActive(true); // แสดง UI
                isPlayerNearby = true; // Player อยู่ใกล้
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าเป็น Player และยังไม่ Trigger มาก่อน
        {
            if (giveKeyText != null)
            {
            giveKeyText.SetActive(false); // ปิด UI
            isPlayerNearby = false; // Player อยู่ไกล
            }
        }
    }

    void Update()
    {
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.E)) // เช็คว่ากดปุ่ม E
        {
            ReviveBot();
        }
    }

    public void GetKeyItem()
    {
        haveKeyItem = true;
        keyItem.SetActive(false);
        dialogText.SetActive(false);
    }

    private void ReviveBot()
    {
        damagedBot.SetActive(false);
        giveKeyText.SetActive(false);
        newBot.SetActive(true);
        StartCoroutine(ChangeScene());
    }

    IEnumerator ChangeScene()
    {
        fade._FadeIn();
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene(4);
    }
    
}
