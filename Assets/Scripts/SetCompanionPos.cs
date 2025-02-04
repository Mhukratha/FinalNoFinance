using UnityEngine;

public class SetCompanionPos : MonoBehaviour
{
    private bool isUsed = false;
    public CompanionFollow companionFollow;
    [SerializeField] private GameObject targetPosition;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // ตรวจสอบว่าเป็น Player หรือไม่
        {
            if (isUsed) return;
            Debug.Log("Player entered the trigger zone!");
            Vector2 targetPos = targetPosition.transform.position;
            companionFollow.TriggerMoveTo(targetPos);
            isUsed = true;
        }
    }
}
