using UnityEngine;
using System.Collections;

public class HideInTrashCan : MonoBehaviour
{
    public Transform hidePosition; // ตำแหน่งซ่อน
    public float shakeDuration = 0.3f; // ระยะเวลาการสั่น
    public float shakeMagnitude = 0.1f; // ความแรงของการสั่น

    private bool isHiding = false;
    private bool nearTrashCan = false;
    private GameObject player;
    private PlayerController playerMovement;
    private SpriteRenderer[] playerRenderers; // ใช้ SpriteRenderer สำหรับ 2D
    private Rigidbody2D playerRigidbody; // ใช้ Rigidbody2D
    private Vector3 originalPosition; // ตำแหน่งเริ่มต้นของถังขยะ

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerController>();
        playerRenderers = player.GetComponentsInChildren<SpriteRenderer>(); // ดึง SpriteRenderer ทุกตัว
        playerRigidbody = player.GetComponent<Rigidbody2D>(); // ดึง Rigidbody2D

        if (player == null)
        {
            Debug.LogError("[HideInTrashCan] Player not found! Make sure the player has the 'Player' tag.");
        }
        if (playerMovement == null)
        {
            Debug.LogError("[HideInTrashCan] PlayerMovement script not found on Player!");
        }
        if (playerRigidbody == null)
        {
            Debug.LogError("[HideInTrashCan] Rigidbody2D not found on Player!");
        }

        originalPosition = transform.position; // เก็บตำแหน่งเริ่มต้นของถังขยะ
    }

    void Update()
    {
        if (nearTrashCan && Input.GetKeyDown(KeyCode.E))
        {
            ToggleHiding();
            if (!isHiding) // ถ้าไม่ได้ซ่อน (จะต้องสั่นหลังจากออกจากการซ่อน)
            {
                StartCoroutine(ShakeTrashCan());
            }
        }
    }

    void ToggleHiding()
    {
        if (!isHiding)
        {
            // ซ่อน Player
            Debug.Log("[HideInTrashCan] Player is hiding.");
            player.transform.position = hidePosition.position;
            playerMovement.enabled = false; // ปิดการเคลื่อนที่
            playerRigidbody.velocity = Vector2.zero; // หยุดความเร็ว
            playerRigidbody.bodyType = RigidbodyType2D.Kinematic; // ป้องกันตกแมพ
            SetPlayerVisible(false); // ซ่อน
        }
        else
        {
            // ออกจากที่ซ่อน
            Debug.Log("[HideInTrashCan] Player is exiting hide.");
            playerMovement.enabled = true; // เปิดการเคลื่อนที่
            playerRigidbody.bodyType = RigidbodyType2D.Dynamic; // เปิดให้ Player เคลื่อนที่ปกติ
            SetPlayerVisible(true); // แสดงตัว
        }
        isHiding = !isHiding;
    }

    void SetPlayerVisible(bool visible)
    {
        foreach (SpriteRenderer rend in playerRenderers)
        {
            rend.enabled = visible;
        }
        Debug.Log("[HideInTrashCan] Player visibility set to: " + visible);
    }

    // ฟังก์ชันสั่นของถังขยะ
    private IEnumerator ShakeTrashCan()
    {
        float elapsedTime = 0f;
        while (elapsedTime < shakeDuration)
        {
            float xOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            float yOffset = Random.Range(-shakeMagnitude, shakeMagnitude);
            transform.position = originalPosition + new Vector3(xOffset, yOffset, 0f);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // คืนตำแหน่งเริ่มต้นหลังจากการสั่น
        transform.position = originalPosition;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            nearTrashCan = true;
            Debug.Log("[HideInTrashCan] Player is near the trash can.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            nearTrashCan = false;
            Debug.Log("[HideInTrashCan] Player left the trash can area.");
        }
    }
}
