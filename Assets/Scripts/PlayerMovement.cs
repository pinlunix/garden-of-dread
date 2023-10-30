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

    [SerializeField] private float jumpForce = 45f;

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheckPoint;    // use as origin
    [SerializeField] private float groundCheckY = 0.2f;     // define distance of ray
    [SerializeField] private float groundCheckX = 0.5f;     // check if player is standing near an edge of platform
    [SerializeField] private LayerMask whatIsGround;        // detect game objects on ground layer

    Rigidbody2D rb;

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

    // Update is called once per frame
    private void FixedUpdate()
    {
        //int jump_scalar = 0;
        //if (gameObject.transform.position.y == 1)
        //{
        //    jump_scalar = 30;
        //}
        Vector2 movement = new Vector2(movementX * 10, movementY * 30);
        rb.AddForce(movement);
    }

    void OnMove(InputValue movementValue)
    {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y;
        Debug.Log(rb.velocity);
        Debug.Log("moving");
    }

    void OnJump()
    {
        if (Grounded())
        {
            rb.velocity = new Vector3(rb.velocity.x, jumpForce);
            Debug.Log("Jumped");
        }
        if (!Grounded() && rb.velocity.y > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
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
