using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic; // ตั้งให้เป็น Kinematic ก่อน เพื่อป้องกันไม่ให้ตกเอง
            rb.gravityScale = 0; // ปิดแรงโน้มถ่วง
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") && !isFalling)
        {
            StartCoroutine(FallAfterDelay(0.5f)); // ให้ดีเลย์ 0.5 วิก่อนตกเฟรม
        }
    }

    IEnumerator FallAfterDelay(float delay)
    {
        isFalling = true;
        yield return new WaitForSeconds(delay); // ให้รอ 0.5 วิ ก่อนพื้นเริ่มตก

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic; // เปลี่ยนให้เป็น Dynamic เพื่อให้ตกลงมาตามแรงโน้มถ่วง
            rb.gravityScale = 2f; // ตั้งค่าแรงโน้มถ่วงเพื่อให้ตกสมูท
        }

        Destroy(gameObject, 2f); // ให้ทำลายพื้นหลังจากตก 2 วินาที
    }
}