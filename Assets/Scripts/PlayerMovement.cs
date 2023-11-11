using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.InputSystem;
using System.Security.Cryptography;
using System.Collections.Specialized;
using UnityEditor;
using System;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Horizontal Movement")]
    public float walkSpeed = 10;
    Vector2 moveDir;
    private float movementX;
    private float movementY;

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

    [Header("Attack Settings")]
    [SerializeField] private Transform AttackTransform;
    [SerializeField] private Vector2 AttackArea;
    [SerializeField] private LayerMask attackableLayer;
    [SerializeField] private float damage;                  // damage player does to enemy

    [Header("References")]
    Rigidbody2D rb;
    private float horizontal, vertical;
    private Animator anim;
    private GameObject attackArea = default;
    private int coincount;
    public int count;

    public static PlayerMovement Instance;

    public CoinManager cm;
    public TextMeshProUGUI coinText;
  

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

        attackArea = transform.GetChild(0).gameObject;
        coincount = 0;
        SetCountText();
        

    }

    private void Update()            // jump movement
    {
        
        // prevent other movements from interrupting Dash
        if (isDashing)
        {
            return;
        }

        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");

        StartDash();
        Flip();

        if (Input.GetButtonUp("Attack"))
        {
            anim.ResetTrigger("isAttacking");
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

   

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(AttackTransform.position, AttackArea);
    }

    // handles moving left or right direction
    private void Flip()
    {
        if(horizontal < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
        }
        else if (horizontal > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
        }
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

        anim.SetBool("isRunning", movementX != 0 && Grounded());

        Debug.Log("moving");
        Debug.Log(rb.velocity.x);
    }
    
    void OnAttack()
    {

        anim.SetTrigger("isAttacking");
        Debug.Log("Attacking");
        Hit(AttackTransform, AttackArea);
    }
    
    void Hit(Transform _attackTransform, Vector2 _attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, attackableLayer);
        
        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(damage);
                Debug.Log("Hit");
            }
        }

    }
    

    void OnJump()
    {
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
            //anim.SetBool("isJumping", true);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }
        anim.SetBool("isJumping", !Grounded());
    }

    public bool Grounded()
    {
        if (Physics2D.Raycast(groundCheckPoint.position, Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround)
            || Physics2D.Raycast(groundCheckPoint.position + new Vector3(-groundCheckX, 0, 0), Vector2.down, groundCheckY, whatIsGround))
        {
            anim.SetBool("isJumping", false);
            return true;
        }
        else
        {
            anim.SetBool("isJumping", true);
            //anim.SetBool("isAttacking", false);
            return false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Coins"))
        {
            Destroy(other.gameObject);
            cm.coinCount++;

            coincount = coincount + 1;
            SetCountText();
           
        }
    }

    void SetCountText()
    {
        coinText.text = "Coins: " + coincount.ToString();
    }

}
