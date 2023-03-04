using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{   
    protected enum EnemyState{
        Idle,
        Attack,
        Stunned
    }

    protected EnemyState enemyState = EnemyState.Idle;
    protected bool a_isAggro = false;
    protected bool a_isStunned = false;


    // Update is called once per frame
    protected virtual void Update()
    {
        switch(enemyState){
            case EnemyState.Idle:
                DoIdle();
                if(a_isStunned){ enemyState = Transition(EnemyState.Stunned); }
                else if(a_isAggro){ enemyState = Transition(EnemyState.Attack); }
                break;

            case EnemyState.Attack:
                DoAttack();
                if(a_isStunned){ enemyState = Transition(EnemyState.Stunned);  }
                else if(!a_isAggro){ enemyState = Transition(EnemyState.Idle); }
                break;

                case EnemyState.Stunned:
                DoStunned();
                if(!a_isStunned){
                    if(!a_isAggro){ enemyState = Transition(EnemyState.Idle); }
                    else{ enemyState = Transition(EnemyState.Attack); }
                }
                break;
        }
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

    //Considering all the aggro parameters check if its true or false
    protected void CheckAggro(bool keepAggro, bool aggroWhenHit, bool aggroOnBothSides,
                                bool hasAggroed, float aggroDistance, Vector2 nextDir,   GameObject target){

        if(keepAggro && hasAggroed || aggroWhenHit && a_isAggro){
            a_isAggro = true;
        }
        else if(aggroOnBothSides){
            if(Check(Vector2.right, aggroDistance, target)){ a_isAggro = true; }
            else{ a_isAggro = Check(Vector2.left, aggroDistance, target); }
        }
        else{ a_isAggro = Check(nextDir, aggroDistance, target);}    //only on the side he's looking/ walking towards
    }

    //lounches a Raycast in search of a target collider
    protected virtual bool Check(Vector2 dir, float distance, GameObject target){  
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
}
