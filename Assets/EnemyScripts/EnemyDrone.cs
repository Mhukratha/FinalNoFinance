using UnityEngine;
using UnityEngine.Rendering.Universal; // ใช้ Light2D
using System.Collections;

public class EnemyLightPatrol2D : MonoBehaviour
{
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

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        if (!playerDetected)
        {
            Patrol();  // ถ้าไม่เจอผู้เล่นให้เดินไป
        }
        DetectPlayer();  // ตรวจสอบการตรวจจับผู้เล่น
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

        foreach (Collider2D hit in hitColliders)
        {
            if (hit.CompareTag(playerTag))  // ตรวจสอบว่า collider นั้นมี Tag ที่ตรงกับ playerTag
            {
                Transform playerTransform = hit.transform;
                Vector2 directionToPlayer = (playerTransform.position - transform.position).normalized;
                float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

                RaycastHit2D raycastHit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

                if (!raycastHit.collider) // ไม่มีสิ่งกีดขวาง
                {
                    if (IsPlayerInSpotLight(playerTransform))
                    {
                        Debug.Log("✅ Player detected in Spot Light!");
                        playerDetected = true;
                        OnPlayerDetected();
                        return; // หยุดตรวจจับต่อ
                    }
                }
                else
                {
                    Debug.Log("⛔ Raycast Blocked by: " + raycastHit.collider.gameObject.name);
                }
            }
        }

        // ถ้าไม่เจอผู้เล่น ให้รีเซ็ตสถานะการตรวจจับ
        if (hitColliders.Length == 0)
        {
            playerDetected = false;
        }
    }

    bool IsPlayerInSpotLight(Transform player)
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float angleToPlayer = Vector2.Angle(transform.right * (movingRight ? 1 : -1), directionToPlayer);

        Debug.Log($"🔦 Angle to Player: {angleToPlayer}, Allowed: {enemyLight.pointLightInnerAngle / 2}");

        return angleToPlayer < enemyLight.pointLightInnerAngle / 2;
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
}
