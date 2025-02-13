using UnityEngine;
using UnityEngine.Rendering.Universal; // ใช้ Light2D
using UnityEngine.SceneManagement;
using System.Collections;

public class EnemyDrone : MonoBehaviour
{
    public GameObject playerPrefab;
    private Transform playerTransform;

    public Light2D enemyLight;
    public float patrolDistance = 3f;
    public float moveSpeed = 2f;
    public float turnDelay = 1f;
    public string playerTag = "Player";
    public LayerMask obstacleLayer;

    private Vector3 startPosition;
    private bool movingRight = true;
    private bool isTurning = false;
    private bool playerDetected = false;

    public float followSpeed = 3f;
    public float rotationSpeed = 5f;
    public float stopDistance = 1.5f;
    public float heightOffset = 1.5f;

    public float lostPlayerTime = 2f;

    private float timeInLight = 0f;
    public float timeToChangeScene = 3f;

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
        DetectPlayer();

        if (playerDetected)
        {
            FollowPlayer();
        }
        else
        {
            Patrol();
        }

        CheckIfInLight();
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
        float newScaleX = movingRight ? -0.15f : 0.15f;
        transform.localScale = new Vector3(newScaleX, transform.localScale.y, transform.localScale.z);

        moveSpeed = 2;
        isTurning = false;
    }

    void DetectPlayer()
    {
        if (playerTransform == null) return;

        // ตรวจสอบว่าผู้เล่นอยู่ในรัศมีของแสงหรือไม่
        if (IsPlayerInSpotLight(playerTransform))
        {
            Debug.Log("✅ Player อยู่ในแสง!");
            playerDetected = true;
            timeInLight += Time.deltaTime; // เพิ่มเวลาที่อยู่ในแสง

            if (timeInLight >= timeToChangeScene)
            {
                Debug.Log("🔄 เปลี่ยนซีน!");
                SceneManager.LoadScene(8); // โหลดซีนถัดไป
            }
        }
        else
        {
            Debug.Log("😎 Player หลบแสง!");
            playerDetected = false;
            timeInLight = 0f; // รีเซ็ตเวลาเมื่อ Player ออกจากแสง
        }
    }


    void StartChangeScene()
    {
        Debug.Log("🔄 เปลี่ยนซีน!");
        SceneManager.LoadScene(0);
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

        if (angleToPlayer > enemyLight.pointLightInnerAngle / 2) return false;

        float distanceToPlayer = Vector2.Distance(enemyLight.transform.position, player.position);
        RaycastHit2D hit = Physics2D.Raycast(enemyLight.transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        Debug.DrawRay(enemyLight.transform.position, directionToPlayer * distanceToPlayer, Color.green, 1f);

        if (hit.collider != null && hit.collider.gameObject != player.gameObject)
        {
            Debug.Log("🛑 Player ถูกบังโดย: " + hit.collider.gameObject.name);
            return false;
        }

        Debug.Log("🔦 Player อยู่ในแสงและไม่มีอะไรกั้น!");
        return true;
    }

    void CheckIfInLight()
    {
        float distanceToLight = Vector2.Distance(transform.position, enemyLight.transform.position);
        if (distanceToLight < enemyLight.pointLightOuterRadius)
        {
            Vector2 directionToLight = (enemyLight.transform.position - transform.position).normalized;
            float angleToLight = Vector2.Angle(transform.right * (movingRight ? 1 : -1), directionToLight);

            if (angleToLight < enemyLight.pointLightInnerAngle / 2)
            {
                if (moveSpeed != 0)
                {
                    Debug.Log("⛔ Stop! Enemy is in the light.");
                    moveSpeed = 0;
                }
            }
            else if (moveSpeed == 0)
            {
                Debug.Log("▶ Resume patrol! Enemy is out of light.");
                moveSpeed = 2;
            }
        }
        else if (moveSpeed == 0)
        {
            Debug.Log("▶ Resume patrol! Enemy is out of light.");
            moveSpeed = 2;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, enemyLight.pointLightOuterRadius);
    }

    void FollowPlayer()
    {
        if (playerTransform == null) return;

        float distanceToPlayerX = Mathf.Abs(playerTransform.position.x - transform.position.x);

        if (distanceToPlayerX > stopDistance)
        {
            float newX = Mathf.MoveTowards(transform.position.x, playerTransform.position.x, followSpeed * Time.deltaTime);
            float newY = Mathf.MoveTowards(transform.position.y, playerTransform.position.y + heightOffset, followSpeed * Time.deltaTime);

            transform.position = new Vector3(newX, newY, transform.position.z);
        }

        float directionX = playerTransform.position.x - transform.position.x;

        if (directionX > 0)
        {
            transform.localScale = new Vector3(-0.15f, 0.15f, 1);
        }
        else if (directionX < 0)
        {
            transform.localScale = new Vector3(0.15f, 0.15f, 1);
        }
    }
}
