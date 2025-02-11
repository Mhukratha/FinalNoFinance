using UnityEngine;
using UnityEditor;
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;          
    public float jumpForce = 10f;     
    public float crouchSpeed = 2.5f;    
    public float slideSpeed = 8f;     
    public float slideDuration = 0.5f; 

    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isCrouching = false;
    private bool isSliding = false;

    private bool isOnZipline = false;
    private Transform zipEndPoint;
    private float zipSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround();
        Crouch();
        Slide();
        Move();
        Jump();
        
         if (isOnZipline)
        {
            transform.position = Vector2.MoveTowards(transform.position, zipEndPoint.position, zipSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, zipEndPoint.position) < 0.1f)
            {
                isOnZipline = false;
                rb.gravityScale = 1;
                Debug.Log("🎯 ");
            }
        }

    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.1f, groundLayer);

         if (isGrounded)
        {
            Debug.Log("Player is on the ground");
        }
        else
        {
            Debug.Log("Player is in the air");
        }
        anim.SetBool("isGrounded", isGrounded);

    }

    void Move()
    {
        if (isSliding) return; 

        float moveInput = Input.GetAxisRaw("Horizontal");
        float moveSpeed = isCrouching ? crouchSpeed : speed;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput) * 0.1f, 0.1f, 0.1f);
            //transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    void Jump()
{
    if (!isGrounded || isCrouching) return; 

    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        anim.SetTrigger("jump");
        isGrounded = false; 
    }
}


    void Crouch()
    {
        if (Input.GetKey(KeyCode.S))
        {
            isCrouching = true;
            anim.SetBool("isCrouching", true);
        }
        else
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
        }
    }

    void Slide()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && isGrounded && !isSliding)
        {
            isSliding = true;
            anim.SetTrigger("slide");

            float slideDirection = transform.localScale.x; 
            rb.linearVelocity = new Vector2(slideSpeed * slideDirection, rb.linearVelocity.y);

            Invoke("EndSlide", slideDuration);
        }
    }

    void EndSlide()
    {
        isSliding = false;
    }
     

     private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Zipline"))
        {
            Debug.Log("🚀 Player Grabbed the Zipline!");

            // ดึงค่าจาก Zipline ที่ Player จับ
            Zipline zipline = collision.GetComponent<Zipline>();
            if (zipline != null)
            {
                zipEndPoint = zipline.endPoint;
                zipSpeed = zipline.zipSpeed;
                isOnZipline = true;

                rb.gravityScale = 0;
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
