using UnityEngine;

public class AudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public float startTime = 5f; // เวลาที่ต้องการให้เสียงเริ่มเล่น (กำหนดใน Inspector)
    
    void Start()
    {
        audioSource.time = startTime; // เริ่มเล่นที่วินาทีที่ 5
        audioSource.Play();
    }
}