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

    private Rigidbody2D rb;
    private Animator anim;
    private bool isGrounded;
    private bool isCrouching = false;
    private bool isSliding = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        Move();
        Jump();
        Crouch();
        Slide();
    }

    void Move()
    {
        if (isSliding) return; 

        float moveInput = Input.GetAxisRaw("Horizontal");
        float moveSpeed = isCrouching ? crouchSpeed : speed;

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
            anim.SetBool("isRunning", true);
        }
        else
        {
            anim.SetBool("isRunning", false);
        }
    }

    void Jump()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);

        if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space)) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            anim.SetTrigger("jump");
        }
    }

    void Crouch()
    {
        if (Input.GetKey(KeyCode.S) && isGrounded)
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
            rb.velocity = new Vector2(slideSpeed * slideDirection, rb.velocity.y);

            Invoke("EndSlide", slideDuration);
        }
    }

    void EndSlide()
    {
        isSliding = false;
    }
}
