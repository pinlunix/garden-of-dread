using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.InputSystem;
using System.Security.Cryptography;
using System.Collections.Specialized;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Horizontal Movement")]
    public float walkSpeed = 10;
    Vector2 moveDir;
    private float movementX;
    private float movementY;
    private bool isFacingRight = true;

    [Header("Vertical Movement")]
    private bool doubleJump;
    private float jumpingPower = 9f;

    [Header("Dash Settings")]
    private bool dashed = false;
    private bool canDash = true;
    private bool isDashing = false;
    private float dashingPower = 24f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 0.2f;


    
    

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;    // use as origin
    [SerializeField] private float groundCheckY = 0.2f;     // define distance of ray
    [SerializeField] private float groundCheckX = 0.5f;     // check if player is standing near an edge of platform
    [SerializeField] private LayerMask whatIsGround;        // detect game objects on ground layer
    [SerializeField] private TrailRenderer tr;

    Rigidbody2D rb;

    private float horizontal;
    private Animator anim;


    public static PlayerMovement Instance;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Update()            // jump movement
    {
        // prevent other movements from interrupting Dash
        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        StartDash();

        if (horizontal > 0 && !isFacingRight)
        {
            Flip();
        }
        if (horizontal < 0 && isFacingRight)
        {
            Flip();
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }

        //Vector2 movement = new Vector2(movementX * 10, movementY * 10);
        //rb.AddForce(movement);
        
        // constant movement, no momentum or sliding
        rb.velocity = new Vector2(horizontal * walkSpeed, rb.velocity.y);
    }

    // handles dashing left or right direction
    private void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1;
        gameObject.transform.localScale = currentScale;

        isFacingRight = !isFacingRight;
    }

    void StartDash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDash && !dashed)
        {
            StartCoroutine(Dash());
            dashed = true;
            Debug.Log("Dashing");
        }

        if (Grounded())
        {
            dashed = false;
        }
    }

    private IEnumerator Dash()                //dash
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0;    // dash forward without falling
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;     // emit trail while dashing
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;    // stop trail emission after dash finishes
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }


    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        Debug.Log("moving");
        Debug.Log(rb.velocity);
        anim.SetBool("isRunning", rb.velocity.x != 0 && Grounded());
    }

    void OnJump()
    {
        /*
        // if player is on the ground
        if (Grounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            Debug.Log("Jumped");
        }
        // if player is not on the ground (GetButtonUp("Jump")) and is moving in y direction (up)
        // jump-cancel
        if (!Grounded() && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
        }
        */

        // If on the ground and not pressing jump
        if (Grounded() && !Input.GetButtonDown("Jump"))
        {
            doubleJump = true;
        }
        // if pressing jump
        if (Input.GetButtonDown("Jump"))
        {
            
            // if on the ground or doubleJump is true
            if (Grounded() || doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

                doubleJump = !doubleJump;
                Debug.Log("Jumped");
            }
        }
        // if jump button released and player is moving on y axis
        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            anim.SetBool("isJumping", !Grounded());
        }
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    
}
