using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollingEnemy : MonoBehaviour
{
    [SerializeField] private float idleTime = 3;
    [SerializeField] private float idleWalkSpeed = 5;
    [SerializeField] private float attackSpeed = 10;
    [SerializeField] private float attackDelay = 3;
    [SerializeField] private float aggroDistance = 10;
    [SerializeField] private bool aggroOnBothSides = false;
    [SerializeField] private bool canDoDamageWhileStopped = true;
    [SerializeField] private bool immuneWhileAttacking = false;
    private EnemyState enemyState = EnemyState.Idle;
    private float nextTime = 0;
    private Vector2 nextDir = Vector2.zero;
    private bool isAggro = false;
    private bool isStunned = false;
    private Rigidbody2D rb;
    private Animator anim;
    private Health health;
    private DoDamage damage;
    private GameObject target;


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

    // Update is called once per frame
    public void Update()
    {
        switch(enemyState){
            case EnemyState.Idle:
                DoIdle();
                if(isStunned){ enemyState = Transition(EnemyState.Stunned); }
                else if(isAggro){ enemyState = Transition(EnemyState.Attack); }
                break;

            case EnemyState.Attack:
                DoAttack();
                if(isStunned){ enemyState = Transition(EnemyState.Stunned);  }
                else if(!isAggro){ enemyState = Transition(EnemyState.Idle); }
                break;

                case EnemyState.Stunned:
                DoStunned();
                if(!isStunned){
                    if(!isAggro){ enemyState = Transition(EnemyState.Idle); }
                    else{ enemyState = Transition(EnemyState.Attack); }
                }
                break;
        }
    }

    enum EnemyState{
        Idle,
        Attack,
        Stunned
    }

    private EnemyState Transition(EnemyState nextState){
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

        //change animation
        anim.SetTrigger("go" + nextState.ToString());
        Debug.Log("NOW_ON -> " + nextState);
        return nextState;
    }

    private void DoIdle(){
        ChooseDir();
        if(aggroOnBothSides){
            if(CheckAggro(Vector2.right)){ isAggro = true; }
            else{ isAggro = CheckAggro(Vector2.left); }
        }else{ isAggro = CheckAggro(nextDir);}    //only on the side he's looking/ walking towards

        //move
        rb.velocity = new Vector2(nextDir.x * idleWalkSpeed, rb.velocity.y);
    }

    private void ChooseDir(){
        if(Time.time > nextTime){
            switch(Random.Range(1,3)){
                case 1:
                    if(nextDir == Vector2.zero || nextDir == Vector2.right){
                        //last time we were either in "stop" or "right"
                        nextDir = Vector2.right;
                        Debug.Log("right");
                    }
                    else{
                        nextDir = Vector2.zero;
                        Debug.Log("stop");
                    }
                    break;
                case 2:
                    if(nextDir == Vector2.zero || nextDir == Vector2.left){
                        //last time we were either in "stop" or "left"
                        nextDir = Vector2.left;
                        Debug.Log("left");
                    }
                    else{
                        nextDir = Vector2.zero;
                        Debug.Log("stop");
                    }
                    break;
            }
            //reset timer
            nextTime = Time.time + Random.Range(idleTime-(idleTime*0.5f), idleTime+(idleTime*0.5f));
        }
    }

    private bool CheckAggro(Vector2 dir){  //lounches a Raycast in search of the player
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, aggroDistance, (1 << 7));
        if(hit && hit.collider.tag == "Player"){
            target = hit.collider.gameObject;
            return true;
        }
        else{return false;}
    }

    private void DoAttack(){
        if(Time.time > nextTime){
            //direction of attack
            nextDir = (target.transform.position - transform.position);

            //I wanted this to be infinite, so it only reset when he hits something
            nextTime = Time.time + 10000000;
        }
        //do attack
        rb.velocity = new Vector2(nextDir.normalized.x * attackSpeed, rb.velocity.y);
    }

    private void DoStunned(){
        if(Time.time > nextTime){
            if(aggroOnBothSides){
                if(CheckAggro(Vector2.right)){ isAggro = true; }
                else{ isAggro = CheckAggro(Vector2.left); }
            }else{ isAggro = CheckAggro(nextDir);}    //only on the side he's looking/ walking towards

            isStunned = false;
        }
    }

    public void Die(){
        Debug.Log("Player Killed -Rolly-");
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("ground")){
            if(enemyState == EnemyState.Idle){
                //hit some obstacle while on Idle mode

                if(other.transform.position.x > transform.position.x){
                    nextDir = Vector2.left;
                }
                else{ nextDir = Vector2.right; }
            }

            if(enemyState == EnemyState.Attack){
                if(other.gameObject.layer != LayerMask.NameToLayer("projectiles")){
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
