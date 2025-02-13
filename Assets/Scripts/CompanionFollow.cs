using UnityEngine;

public class CompanionFollow : MonoBehaviour
{
    public Transform player;  // ตัวละครหลัก
    public float followSpeed = 5f;
    public float stoppingDistance = 1f;
    public float missionCompleteDelay = 2f; // เวลาหน่วงก่อนกลับหาผู้เล่น
    public float verticalOffset = 1f; // ความสูงที่ Companion อยู่เหนือ Player

    private Rigidbody2D rb;
    private Vector2 movement;
    private Vector2 targetPosition;
    private bool isCustomTargetSet = false;
    [SerializeField] private SpriteRenderer spriteRenderer;

    private enum CompanionState { FollowingPlayer, GoingToTarget, ReturningToPlayer }
    private CompanionState currentState = CompanionState.FollowingPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        switch (currentState)
        {
            case CompanionState.FollowingPlayer:
                FollowPlayer();
                break;
            case CompanionState.GoingToTarget:
                MoveToTarget(targetPosition, CompanionState.ReturningToPlayer);
                break;
            case CompanionState.ReturningToPlayer:
                FollowPlayer();
                break;
        }

        if (movement.x > 0)
            //transform.localScale = new Vector3(1, 1, 1);  
            spriteRenderer.flipX = false; // หันไปทางขวา
        else if (movement.x < 0)
            //transform.localScale = new Vector3(-1, 1, 1); 
            spriteRenderer.flipX = true; // หันไปทางซ้าย

    }

    void FixedUpdate()
    {
        if (movement != Vector2.zero)
        {
            rb.MovePosition(rb.position + movement * followSpeed * Time.fixedDeltaTime);
        }
    }

    void FollowPlayer()
    {
        if (isCustomTargetSet) return; // หยุดเดินตาม Player ถ้ามีเป้าหมายใหม่

        Vector2 playerTarget = GetPlayerPositionWithOffset();

        float distance = Vector2.Distance(transform.position, playerTarget);
        if (distance > stoppingDistance)
        {
            movement = (playerTarget - (Vector2)transform.position).normalized;
        }
        else
        {
            movement = Vector2.zero;
        }
    }

    void MoveToTarget(Vector2 target, CompanionState nextState)
    {
        
        float distance = Vector2.Distance(transform.position, target);
        if (distance > 0.5f)
        {
            movement = (target - (Vector2)transform.position).normalized;
        }
        else
        {
            movement = Vector2.zero;
            StartCoroutine(WaitAndChangeState(nextState));
        }
    }

    System.Collections.IEnumerator WaitAndChangeState(CompanionState nextState)
    {
        yield return new WaitForSeconds(missionCompleteDelay);
        isCustomTargetSet = false;
        currentState = nextState;
    }

    // Method เอาไว้เรียกจาก Event หรือ Script อื่น
    public void TriggerMoveTo(Vector2 newPosition)
    {
        targetPosition = newPosition;
        isCustomTargetSet = true;
        currentState = CompanionState.GoingToTarget;
    }

    // คำนวณตำแหน่ง Player พร้อม Offset ความสูง
    private Vector2 GetPlayerPositionWithOffset()
    {
        return new Vector2(player.position.x, player.position.y + verticalOffset);
    }
}
