using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class FlyingMob : Enemy
{
    [SerializeField] private float chaseDistance;
    [SerializeField] private float stunDuration;

    float timer;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.FlyingMob_Idle);
    }

    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerMovement.Instance.transform.position);

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.FlyingMob_Idle:
                if(_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.FlyingMob_Chase);
                }
                break;
            case EnemyStates.FlyingMob_Chase:
                rb.MovePosition(Vector2.MoveTowards(transform.position, PlayerMovement.Instance.transform.position, Time.deltaTime * speed));
                FlipFlyingMob();
                break;
            case EnemyStates.FlyingMob_Stunned:
                timer += Time.deltaTime;

                if(timer > stunDuration)
                {
                    ChangeState(EnemyStates.FlyingMob_Idle);
                    timer = 0;
                }
                break;
            case EnemyStates.FlyingMob_Death:
                break;
        }
    }

    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce){
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        if(health > 0){
            ChangeState(EnemyStates.FlyingMob_Stunned);
        }
        else{
            ChangeState(EnemyStates.FlyingMob_Death);
        }
    }

    protected override void ChangeCurrentAnimation(){

    }

    void FlipFlyingMob()
    {
        sr.flipX = PlayerMovement.Instance.transform.position.x < transform.position.x;
    }
    
}
