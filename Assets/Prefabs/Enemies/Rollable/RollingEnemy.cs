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
    private EnemyState enemyState = EnemyState.idle;
    private float nextTime = 0;
    private Vector2 nextDir = Vector2.zero;
    private bool isAggro = false;
    private bool isStunned = false;
    private Rigidbody2D rb;
    private GameObject target;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void Update()
    {
        switch(enemyState){
            case EnemyState.idle:
                DoIdle();
                if(isAggro){
                    enemyState = EnemyState.attack;
                    nextTime = 0;
                    nextDir = Vector2.zero;
                    Debug.Log("ATTACK_MODE");
                }
                break;

            case EnemyState.attack:
                DoAttack();
                if(!isAggro){
                    enemyState = EnemyState.idle;
                    nextTime = 0;
                    nextDir = Vector2.zero;
                    Debug.Log("IDLE_MODE");
                }
                break;
        }
    }

    enum EnemyState{
        idle,
        attack
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
            Debug.Log("Found you, now Aggro");
            target = hit.collider.gameObject;
            return true;
        }
        else{return false;}
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.layer != 6){
            if(enemyState == EnemyState.idle){
                //hit some obstacle while on Idle mode

                if(other.transform.position.x > transform.position.x){
                    nextDir = Vector2.left;
                }
                else{ nextDir = Vector2.right; }
            }

            if(enemyState == EnemyState.attack){
                //hit some obstacle while on Attack mode
                
                rb.velocity = new Vector2(0,rb.velocity.y);
                isStunned = true;

                //reset timer
                nextTime = Time.time + attackDelay;
            }
        }
    }

    private void DoAttack(){

        if(Time.time > nextTime){

            if(!isStunned){
                //direction of attack
                nextDir = (target.transform.position - transform.position);

                //I wanted this to be infinite, so it only reset when he hits something
                nextTime = Time.time + 10000000;    
            }
            else{   //when stunned check for aggro
                if(aggroOnBothSides){
                    if(CheckAggro(Vector2.right)){ isAggro = true; }
                    else{ isAggro = CheckAggro(Vector2.left); }
                }else{ isAggro = CheckAggro(nextDir);}    //only on the side he's looking/ walking towards
            }

            isStunned = false;  //resetting
        }

        if(!isStunned){
            //do attack
            rb.velocity = new Vector2(nextDir.normalized.x * attackSpeed, rb.velocity.y);
        }
    }

    public void Die(){
        Debug.Log("Player Killed -Rolly-");
        Destroy(gameObject);
    }
}
