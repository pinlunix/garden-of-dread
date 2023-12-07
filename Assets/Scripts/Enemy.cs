using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;
    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected float speed;

    [SerializeField] protected float damage;
    [SerializeField] protected float knockbackForce = 15f;
    
    protected Vector2 collideDir;
    protected float recoilTimer;
    protected Rigidbody2D rb;
    private Animator anim;

    protected enum EnemyStates
    {
        // GreenMob
        GreenMob_Idle,
        GreenMob_Flip
    }

    protected EnemyStates currentEnemyState;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateEnemyStates();

        if(health <= 0)
        {
            Destroy(gameObject);
        }

        if (isRecoiling)
        {
            if (recoilTimer < recoilLength)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer= 0;
            }
        }
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        if (!isRecoiling)
        {
            rb.AddForce(-_hitForce * recoilFactor * _hitDirection);
        }
    }

    protected void OnTriggerStay2D(Collider2D _other)
    {
        collideDir = (_other.transform.position - transform.position).normalized;
        if(_other.CompareTag("Player") && !PlayerMovement.Instance.invincible)
        {            
            Attack();
            anim.SetBool("isAttacking", true);
        }
    }

    protected virtual void UpdateEnemyStates()
    {

    }

    protected void ChangeState(EnemyStates _newState)
    {
        currentEnemyState = _newState;
    }

    protected virtual void Attack()
    {
        Vector2 knockback = collideDir * knockbackForce;
        PlayerMovement.Instance.TakeDamage(damage, knockback);
    }
}
