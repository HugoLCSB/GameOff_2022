using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserEnemy : Enemy
{
    [SerializeField] private float idleTime = 3;
    [SerializeField] private float idleWalkSpeed = 5;
    [SerializeField] private float attackSpeed = 10;
    [SerializeField] private float attackDelay = 3;
    [SerializeField] private float attackDistance = 3;
    [SerializeField] private float aggroDistance = 10;
    [SerializeField] private bool aggroOnBothSides = false;
    [SerializeField] private bool aggroWhenHit = false;
    [SerializeField] private bool keepAggro = false;
    [SerializeField] private bool immuneWhileAttacking = false;

    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject laser;
    [SerializeField] private GameObject target; 

    private float nextTime = 0;
    private Vector2 nextDir = Vector2.zero;
    private bool hasAggroed = false;

    private Rigidbody2D rb;
    private Animator anim;
    private Health health;
    private FlipOnMovement flip;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if(immuneWhileAttacking){
            health = GetComponent<Health>();
        }

        flip = GetComponent<FlipOnMovement>();
    }

    protected override EnemyState Transition(EnemyState nextState){
        if(nextState == EnemyState.Idle || nextState == EnemyState.Attack){
            //resetting
            nextTime = 0;
            nextDir = Vector2.zero;
        }

        if(nextState == EnemyState.Idle || nextState == EnemyState.Stunned){
             flip.SetTarget(null);  //deselecting target so he flips on movement

            if(immuneWhileAttacking && health != null){ 
                health.Immunity(false);
            }
        }
        else{   //attack
            flip.SetTarget(target.transform);   //deselecting target so he flips towards him

            if(immuneWhileAttacking && health != null){ 
                health.Immunity(true);
            }
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
        if(keepAggro && hasAggroed || aggroWhenHit && isAggro){
            isAggro = true;
        }
        else if(aggroOnBothSides){
            if(CheckAggro(Vector2.right, aggroDistance, target)){ isAggro = true; }
            else{ isAggro = CheckAggro(Vector2.left, aggroDistance, target); }
        }
        else{ isAggro = CheckAggro(nextDir, aggroDistance, target);}    //only on the side he's looking/ walking towards
    }

    protected override void DoAttack(){
        if(Time.time > nextTime && (Vector2.Distance(target.transform.position, transform.position) <= attackDistance)){
             anim.SetTrigger("shoot");

            //Recoil (?)

            //creating bullets at the firePoint and adding velocity to them
            GameObject shotInstance =  Instantiate(laser, firePoint.position, Quaternion.identity);
            
            Vector2 newDir = new 
            Vector2(target.transform.position.x - transform.position.x, 0).normalized;

            if(newDir.x < 0){
                // Multiply the player's x local scale by -1.
                Vector3 theScale = shotInstance.transform.localScale;
                theScale.x *= -1;
                shotInstance.transform.localScale = theScale;
            }

            shotInstance.GetComponent<BulletHandler>().setDir(newDir);

            nextTime = Time.time + attackDelay;
        }

        //move closer to player
        if(Vector2.Distance(target.transform.position, transform.position) > attackDistance){
            nextDir = (target.transform.position - transform.position).normalized;
            rb.velocity = new Vector2(nextDir.x * attackSpeed, rb.velocity.y);
        }
        else if(Vector2.Distance(target.transform.position, transform.position) < attackDistance -2){
            nextDir = (transform.position - target.transform.position).normalized;
            rb.velocity = new Vector2(nextDir.x * attackSpeed, rb.velocity.y);
        }

        if(rb.velocity.x != 0){ anim.SetBool("isMoving", true); }
        else{ anim.SetBool("isMoving", false); }
    }

    protected override void DoStunned(){
        if(Time.time > nextTime){
            //check for transition to next state
            if(keepAggro && hasAggroed || aggroWhenHit && isAggro){
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
            if(other.transform.position.x > transform.position.x){
                nextDir = Vector2.left;
            }
            else{ nextDir = Vector2.right; }

            
            if(other.gameObject.layer == LayerMask.NameToLayer("projectiles")){
                //hit some obstacle while on Attack mode
                //Debug.Log(LayerMask.LayerToName(other.gameObject.layer));

                if(aggroWhenHit){ isAggro = true; }
            
                //rb.velocity = new Vector2(0,rb.velocity.y);
                isStunned = true;

                //reset timer
                nextTime = Time.time + attackDelay;
            }
        }
    }
}
