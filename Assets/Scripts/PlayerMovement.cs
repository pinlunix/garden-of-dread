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

    [SerializeField] private float jumpForce = 10;

    private bool doubleJump;
        
    private bool canDash = true;
    private bool isDashing;
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
    private float jumpingPower = 16f;

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
    }

    private void Update()            // jump movement
    {
        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");

        if (Grounded() && !Input.GetButton("Jump"))
        {
            doubleJump = false;
         
        }

        if (Input.GetButtonDown("Jump"))
        {
            if (Grounded() || doubleJump)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

                doubleJump = !doubleJump;
                Debug.Log("Jumped");
            }
        }

        if (Input.GetButtonUp("Jump") && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
            
       if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

   //void OnJump()
    //{
    //    if (Grounded())
    //    {
    //        rb.velocity = new Vector3(rb.velocity.x, jumpForce);
    //        Debug.Log("Jumped");
    //    }
    //    if (!Grounded() && rb.velocity.y > 0)
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x, 0);
    //    }
    //}

    private IEnumerator Dash()                //dash
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 2;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = 2;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        //int jump_scalar = 0;
        //if (gameObject.transform.position.y == 1)
        //{
        //    jump_scalar = 10;
        //}
        Vector2 movement = new Vector2(movementX * 10, movementY * 10);
        rb.AddForce(movement);

        if (isDashing)
        {
            return;
        }
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        Debug.Log(rb.velocity);
        Debug.Log("moving");
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
