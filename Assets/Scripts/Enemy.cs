using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using UnityEngine.InputSystem;

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
    protected SpriteRenderer sr;
    protected private float resetAttackTimer;

    protected enum EnemyStates
    {
        // GreenMob
        GreenMob_Idle,
        GreenMob_Flip,

        // FlyingMob
        FlyingMob_Idle,
        FlyingMob_Chase,
        FlyingMob_Stunned,
        FlyingMob_Death
    }

    protected EnemyStates currentEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState{
        get {
            return currentEnemyState;
        }
        set{
            if(currentEnemyState != value){
                currentEnemyState = value;
                ChangeCurrentAnimation();
            }
        }
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        resetAttackTimer = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        resetAttackTimer += Time.deltaTime;
        if (resetAttackTimer > 3.0f && anim.GetBool("isAttacking")){
            anim.ResetTrigger("isAttacking");
            resetAttackTimer = 0;
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
        else{
            UpdateEnemyStates();
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
        }
    }

    protected virtual void Death(float _destroyTime){
        Destroy(gameObject, _destroyTime);
    }

    protected virtual void UpdateEnemyStates()
    {

    }

    protected virtual void ChangeCurrentAnimation(){

    }

    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }

    protected virtual void Attack()
    {
        Vector2 knockback = collideDir * knockbackForce;
        PlayerMovement.Instance.TakeDamage(damage, knockback);
        anim.SetTrigger("isAttacking");
    }
}
