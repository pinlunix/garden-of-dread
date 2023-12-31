using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    Vector2 startPos;
    Rigidbody2D playerRb;
    public HealthBar healthBarScript;

    private void Awake()
    {
        playerRb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Thorns"))
        {
            Die();
        }
        if (collision.CompareTag("Enemy"))
        {
            Die();
        }
    }

    public void Die()
    {
        StartCoroutine(Respawn(0.5f));
    }

    public IEnumerator Respawn(float duration)
    {
        playerRb.simulated = false;
        transform.localScale = new Vector3(0, 0, 0);
        yield return new WaitForSeconds(duration);
        transform.position = startPos;
        transform.localScale = new Vector3(1, 1, 1);
        playerRb.simulated = true;

        //reset Player health
        PlayerMovement.Instance.health = PlayerMovement.Instance.maxHealth;
        healthBarScript.SetMaxHealth(PlayerMovement.Instance.maxHealth);
    }
}
