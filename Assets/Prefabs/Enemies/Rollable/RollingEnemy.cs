using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : Enemy
{
    [SerializeField] private float idleTime = 3;
    [SerializeField] private float idleWalkSpeed = 5;
    [SerializeField] private float attackSpeed = 10;
    [SerializeField] private float attackDelay = 3;
    [SerializeField] private float aggroDistance = 10;
    [SerializeField] private bool aggroOnBothSides = false;
    [SerializeField] private bool keepAggro = false;
    [SerializeField] private bool canDoDamageWhileStopped = true;
    [SerializeField] private bool immuneWhileAttacking = false;

    [SerializeField] private GameObject target;

    private Vector2 nextDir = Vector2.zero;
    private bool hasAggroed = false;
    private float nextTime = 0;

    private Rigidbody2D rb;
    private Animator anim;
    private Health health;
    private DoDamage damage;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        if(immuneWhileAttacking){
            health = GetComponent<Health>();
        }
        if(!canDoDamageWhileStopped){
            damage = GetComponent<DoDamage>();
            damage.isOn(false);
        }
    }

    protected override EnemyState Transition(EnemyState nextState){
        if(nextState == EnemyState.Idle || nextState == EnemyState.Attack){
            //resetting
            nextTime = 0;
            nextDir = Vector2.zero;
        }

        //handling immunity while attacking and damage while stopped
        if(immuneWhileAttacking && health != null){ 
            if(nextState == EnemyState.Idle || nextState == EnemyState.Stunned){
                health.Immunity(false);
            }
            else{ health.Immunity(true); } //when attack
        }
        if(!canDoDamageWhileStopped){
            if(nextState == EnemyState.Idle || nextState == EnemyState.Stunned){
                   damage.isOn(false);
            }
            else{ damage.isOn(true); } //when attack
        }

        //setting up continuos aggro from this point on
        if(isAggro){ hasAggroed = true; }

        //change animation
        anim.SetTrigger("go" + nextState.ToString());
        Debug.Log("NOW_ON -> " + nextState);
        return nextState;
    }

   protected override void DoIdle(){
        if(Time.time > nextTime){
            nextDir = ChooseHorizontalDir(nextDir);

            //reset timer
            nextTime = Time.time + Random.Range(idleTime-(idleTime*0.5f), idleTime+(idleTime*0.5f));
        } 

        //move
        rb.velocity = new Vector2(nextDir.x * idleWalkSpeed, rb.velocity.y);

        if(nextDir.x != 0){ anim.SetBool("isMoving", true); }
        else{ anim.SetBool("isMoving", false); }

        //check for transition to next state
            if(keepAggro && hasAggroed){
                isAggro = true;
            }
            else if(aggroOnBothSides){
                if(CheckAggro(Vector2.right, aggroDistance, target)){ isAggro = true; }
                else{ isAggro = CheckAggro(Vector2.left, aggroDistance, target); }
            }
            else{ isAggro = CheckAggro(nextDir, aggroDistance, target);}    //only on the side he's looking/ walking towards
    }

    protected override void DoAttack(){
        if(Time.time > nextTime){
            //direction of attack
            nextDir = (target.transform.position - transform.position);

            //I wanted this to be infinite, so it only reset when he hits something
            nextTime = Time.time + 10000000;
        }
        //do attack
        rb.velocity = new Vector2(nextDir.normalized.x * attackSpeed, rb.velocity.y);
    }

    protected override void DoStunned(){
        if(Time.time > nextTime){
            //check for transition to next state
            if(keepAggro && hasAggroed){
                isAggro = true;
            }
            else if(aggroOnBothSides){
                if(CheckAggro(Vector2.right, aggroDistance, target)){ isAggro = true; }
                else{ isAggro = CheckAggro(Vector2.left, aggroDistance, target); }
            }
            else{ isAggro = CheckAggro(nextDir, aggroDistance, target);}    //only on the side he's looking/ walking towards

            isStunned = false;
        }
    }

    protected override void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("ground")){
            if(enemyState == EnemyState.Idle){
                //hit some obstacle while on Idle mode

                if(other.transform.position.x > transform.position.x){
                    nextDir = Vector2.left;
                }
                else{ nextDir = Vector2.right; }
            }

            if(other.gameObject.layer == LayerMask.NameToLayer("projectiles")){
                if(enemyState != EnemyState.Attack){
                    isStunned = true;

                    //reset timer
                    nextTime = Time.time + attackDelay;
                }
            }
            else{
                if(enemyState == EnemyState.Attack){
                    //hit some obstacle while on Attack mode
                    Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
                
                    rb.velocity = new Vector2(0,rb.velocity.y);
                    isStunned = true;

                    //reset timer
                    nextTime = Time.time + attackDelay;
                }
            }
        }
    }
}
