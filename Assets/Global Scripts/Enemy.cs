using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{   
    protected EnemyState enemyState = EnemyState.Idle;
    protected bool isAggro = false;
    protected bool isStunned = false;


    // Update is called once per frame
    protected virtual void Update()
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

    protected enum EnemyState{
        Idle,
        Attack,
        Stunned
    }

    protected abstract EnemyState Transition(EnemyState nextState);

    protected abstract void DoIdle();

    //randomly chooses what to do next, sets nextDir
    protected virtual Vector2 ChooseHorizontalDir(Vector2 prevDir){
        Vector2 nextDir = Vector2.zero;
        switch(Random.Range(1,3)){
            case 1:
                if(prevDir == Vector2.zero || prevDir == Vector2.right){
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
                if(prevDir == Vector2.zero || prevDir == Vector2.left){
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
        return nextDir;
    }

    //lounches a Raycast in search of a collider
    protected virtual bool CheckAggro(Vector2 dir, float distance, GameObject target){  
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distance, (1 << 7));
        if(hit && hit.transform.gameObject == target){ return true; }
        else{ return false; }
    }

    protected abstract void DoAttack();

    protected abstract void DoStunned();

    public virtual void Die(){
        Debug.Log("Player Killed " + this.gameObject.name);
        Destroy(gameObject);
    }

    protected abstract void OnCollisionEnter2D(Collision2D other);
}
