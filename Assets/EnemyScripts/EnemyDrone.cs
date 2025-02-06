using UnityEngine;
using UnityEngine.Rendering.Universal; // ใช้ Light2D
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyDrone : MonoBehaviour
{
    public GameObject playerPrefab; // เก็บ Prefab ของ Player
    private Transform playerTransform; // เก็บตำแหน่งของ Player ในเกม

    public Light2D enemyLight;          // Spot Light 2D
    public float patrolDistance = 3f;   // ระยะเดิน
    public float moveSpeed = 2f;        // ความเร็วเดิน
    public float turnDelay = 1f;        // เวลาหยุดก่อนหันกลับ
    public string playerTag = "Player"; // Tag ที่ใช้สำหรับผู้เล่น
    public LayerMask obstacleLayer;     // เลเยอร์สิ่งกีดขวาง

    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isTurning = false;
    private bool playerDetected = false;

    public float followSpeed = 3f;  // ความเร็วในการบิน
    public float rotationSpeed = 5f; // ความเร็วในการหันหน้า
    public float stopDistance = 1.5f; // ระยะห่างจาก Player ที่ศัตรูจะหยุดบิน
    public float heightOffset = 1.5f; // ความสูงที่ศัตรูจะบินเหนือ Player

    private float timeSinceLastSeen = 0f; // เวลานับถอยหลังเมื่อไม่เห็น Player
    public float lostPlayerTime = 2f; // เวลาที่จะรอก่อนกลับไปที่จุดเริ่มต้น

    private float timeInLight = 0f; // เวลาที่ Player อยู่ในแสง
    public float timeToChangeScene = 3f; // กำหนดเวลาก่อนเปลี่ยนซีน



    private void Start()
    {
        startPosition = transform.position;

        // ค้นหา Player ที่เกิดจาก Prefab
        if (playerPrefab != null)
        {
            GameObject playerInstance = GameObject.Find(playerPrefab.name);
            if (playerInstance != null)
            {
                playerTransform = playerInstance.transform;
            }
            else
            {
                Debug.LogWarning("⚠️ ไม่พบ Player ที่สร้างจาก Prefab!");
            }
        }
    }


    private void Update()
    {
        DetectPlayer();  // ตรวจจับ Player

        if (playerDetected)
        {
            FollowPlayer(); // บินตาม Player ถ้าเจอ
        }
        else
        {
            Patrol(); // ถ้ายังไม่เจอ Player ให้เดินปกติ
        }

        CheckIfInLight(); // ตรวจสอบว่าศัตรูอยู่ในพื้นที่แสงหรือไม่
    }

    void Patrol()
    {
        if (isTurning) return;

        float moveDirection = movingRight ? 1 : -1;
        transform.position += new Vector3(moveSpeed * moveDirection * Time.deltaTime, 0f, 0f);

        float distanceFromStart = Mathf.Abs(transform.position.x - startPosition.x);
        if (distanceFromStart >= patrolDistance)
        {
            StartCoroutine(TurnAround());
        }
    }

    IEnumerator TurnAround()
    {
        isTurning = true;
        moveSpeed = 0;
        yield return new WaitForSeconds(turnDelay);

        movingRight = !movingRight;

        // ตั้งค่า localScale = 0.15
        float newScaleX = movingRight ? -0.15f : 0.15f;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);

        moveSpeed = 2;
        isTurning = false;
    }

    void DetectPlayer()
    {
        // ค้นหาผู้เล่นในรัศมีของ Spot Light
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, enemyLight.pointLightOuterRadius);

        if (playerTransform == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer <= enemyLight.pointLightOuterRadius)
        {
            Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer);

            if (hit.collider != null && hit.collider.gameObject == playerTransform.gameObject)
            {
                Debug.Log("✅ พบ Player!");
                playerDetected = true;
                timeInLight += Time.deltaTime; // นับเวลาที่ Player อยู่ในแสง

                if (timeInLight >= timeToChangeScene)
                {
                    Debug.Log("🔄 เปลี่ยนซีน!");
                    SceneManager.LoadScene(0); // แก้เป็นชื่อซีนที่ต้องการ
                }
            }
            else
            {
                playerDetected = false;
                timeInLight = 0f; // รีเซ็ตเวลาเมื่อ Player ออกจากแสง
            }
        }
        else
        {
            playerDetected = false;
            timeInLight = 0f; // รีเซ็ตเวลาเมื่อ Player ออกจากระยะ
        }

        if (playerTransform == null) return;

        if (IsPlayerInSpotLight(playerTransform))
        {
            Debug.Log("✅ Player ถูกจับได้ในแสง!");
            playerDetected = true;
            timeInLight += Time.deltaTime;

            if (timeInLight >= timeToChangeScene)
            {
                Debug.Log("🔄 เปลี่ยนซีน!");
                SceneManager.LoadScene(0);
            }
        }
        else
        {
            Debug.Log("😎 Player หลบอยู่ในเงาหรือมีสิ่งกีดขวาง!");
            playerDetected = false;
            timeInLight = 0f;
        }
    }

    void StartLosingPlayer()
    {
        if (playerDetected)
        {
            timeSinceLastSeen += Time.deltaTime;

            if (timeSinceLastSeen >= lostPlayerTime)
            {
                Debug.Log("⏳ ไม่พบ Player เกิน 2 วิ กลับจุดเดิม");
                playerDetected = false;
                ReturnToStart();
            }
        }
    }

    void ReturnToStart()
    {
        StopAllCoroutines();
        StartCoroutine(MoveToStartPosition());
    }

    IEnumerator MoveToStartPosition()
    {
        while (Vector2.Distance(transform.position, startPosition) > 0.1f)
        {
            transform.position = Vector2.MoveTowards(transform.position, startPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }

        Debug.Log("🚀 กลับมาที่จุดเดิม!");
    }

    bool IsPlayerInSpotLight(Transform player)
    {
        Vector2 directionToPlayer = (player.position - enemyLight.transform.position).normalized;
        float angleToPlayer = Vector2.Angle(enemyLight.transform.up, directionToPlayer);

        // ตรวจสอบว่า Player อยู่ในกรวยแสงของโดรนหรือไม่
        if (angleToPlayer > enemyLight.pointLightInnerAngle / 2) return false;

        float distanceToPlayer = Vector2.Distance(enemyLight.transform.position, player.position);

        // ใช้ Raycast เช็คว่ามีสิ่งกีดขวางระหว่างแสงกับ Player ไหม
        RaycastHit2D hit = Physics2D.Raycast(enemyLight.transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        // วาดเส้น Raycast เพื่อดูผลการตรวจสอบ
        Debug.DrawRay(enemyLight.transform.position, directionToPlayer * distanceToPlayer, Color.red, 0.1f);

        if (hit.collider != null && hit.collider.gameObject != player.gameObject)
        {
            Debug.Log("🛑 Player ถูกบังโดย: " + hit.collider.gameObject.name);
            return false; // มีสิ่งกีดขวาง → Player ซ่อนตัวได้
        }

        Debug.Log("🔦 Player อยู่ในแสงและไม่มีอะไรกั้น!");
        return true;
    }

    void OnPlayerDetected()
    {
        moveSpeed = 0; // หยุดเดินเมื่อพบผู้เล่น
    }

    void CheckIfInLight()
    {
        // ตรวจสอบว่าศัตรูอยู่ในพื้นที่แสงของ Spot Light
        float distanceToLight = Vector2.Distance(transform.position, enemyLight.transform.position);
        if (distanceToLight < enemyLight.pointLightOuterRadius)
        {
            // เช็คมุมของศัตรูกับแสง
            Vector2 directionToLight = (enemyLight.transform.position - transform.position).normalized;
            float angleToLight = Vector2.Angle(transform.right * (movingRight ? 1 : -1), directionToLight);

            if (angleToLight < enemyLight.pointLightInnerAngle / 2)
            {
                if (moveSpeed != 0)  // ถ้าไม่หยุดอยู่แล้ว
                {
                    Debug.Log("⛔ Stop! Enemy is in the light.");
                    moveSpeed = 0;  // หยุดเดิน
                }
            }
            else if (moveSpeed == 0)
            {
                // ถ้าออกจากแสง ให้เริ่มเดินต่อ
                Debug.Log("▶ Resume patrol! Enemy is out of light.");
                moveSpeed = 2; // เริ่มเดินต่อ
            }
        }
        else if (moveSpeed == 0)
        {
            // ถ้าออกจากแสงทั้งหมด ให้เริ่มเดินต่อ
            Debug.Log("▶ Resume patrol! Enemy is out of light.");
            moveSpeed = 2; // เริ่มเดินต่อ
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyLight.pointLightOuterRadius);
    }

    void FollowPlayer()
    {
        if (playerTransform == null) return; // ถ้าไม่มี Player ไม่ต้องทำอะไร

        // คำนวณระยะห่างระหว่างศัตรูกับ Player (เฉพาะแกน X)
        float distanceToPlayerX = Mathf.Abs(playerTransform.position.x - transform.position.x);

        // ถ้าศัตรูยังอยู่ไกลกว่า stopDistance ให้บินเข้าไปหา Player (เฉพาะแกน X)
        if (distanceToPlayerX > stopDistance)
        {
            float newX = Mathf.MoveTowards(transform.position.x, playerTransform.position.x, followSpeed * Time.deltaTime);
            float newY = Mathf.MoveTowards(transform.position.y, playerTransform.position.y + heightOffset, followSpeed * Time.deltaTime);

            transform.position = new Vector3(newX, newY, transform.position.z); // อัปเดตตำแหน่ง
        }

        // คำนวณทิศทางของ Player
        float directionX = playerTransform.position.x - transform.position.x;

        // หันหน้าไปทาง Player โดยคงขนาดไว้ที่ 0.15
        if (directionX > 0)
        {
            transform.localScale = new Vector3(-0.15f, 0.15f, 1); // หันขวา
        }
        else if (directionX < 0)
        {
            transform.localScale = new Vector3(0.15f, 0.15f, 1); // หันซ้าย
        }
    }
}
