using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    public float walkSpeed = 10;
    Vector2 moveDir;
    Rigidbody2D rb;
    private float movementX;
    private float movementY;

    [SerializeField] private float jumpForce = 45;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void OnMove(InputValue movementValue) {
        Vector2 movementVector = movementValue.Get<Vector2>();
        movementX = movementVector.x;
        movementY = movementVector.y; 
    }


    private void FixedUpdate()
    {

        int jump_scalar = 0;
        if (gameObject.transform.position.y == 1)
        {
            jump_scalar = 30;
        }
        Vector2 movement = new Vector2(movementX * 10, movementY * 30);
        rb.AddForce(movement);
    }

    
}
