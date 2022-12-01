using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class FlyingEnemy : Enemy
{
    [SerializeField] private float nextWayPointDistance = 3f;
    [SerializeField] private float attackSpeed = 200;
    [SerializeField] private float attackDashForce = 200;
    [SerializeField] private float attackDistance = 3;
    [SerializeField] private float attackDelay = 3;
    [SerializeField] private float idleTime = 3;
    [SerializeField] private float idleWalkSpeed = 5;
    [SerializeField] private float aggroDistance = 10;
    [SerializeField] private bool aggroOnBothSides = false;
    [SerializeField] private bool aggroWhenHit = false;
    [SerializeField] private bool keepAggro = false;

    [SerializeField] private Collider2D spearAreaOfEffect;
    [SerializeField] private GameObject target;

    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;

     private Vector2 nextDir = Vector2.zero;
    private float nextTime = 0;
    private bool hasAggroed = false;

    private Seeker seeker;
    private Rigidbody2D rb;
    private Animator anim;

    private void Start() {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        spearAreaOfEffect.enabled = false;

        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    protected override EnemyState Transition(EnemyState nextState){
        if(nextState == EnemyState.Idle || nextState == EnemyState.Attack){
            //resetting
            nextTime = 0;
            nextDir = Vector2.zero;
        }

        //when attack always look at the target
        if(nextState == EnemyState.Attack){
            if(TryGetComponent<FlipOnMovement>(out FlipOnMovement flip)){
                flip.SetTarget(target.transform);
            }
        }
        else{
            if(TryGetComponent<FlipOnMovement>(out FlipOnMovement flip)){
                flip.SetTarget(null);
            }
        }

        //reset rotation
        transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,0f));

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
        if(Vector2.Distance(transform.position, target.transform.position) <= attackDistance){
            if(Time.time > nextTime){
                //direction of attack
                RotateTowards();

                rb.AddForce((target.transform.position - transform.position).normalized * attackDashForce);
                anim.SetTrigger("attackSpear");
                spearAreaOfEffect.enabled = true;

                //I wanted this to be infinite, so it only reset when he hits something
                nextTime = Time.time + attackDelay;
            }
            else if(!anim.GetCurrentAnimatorStateInfo(0).IsName("FlyEnemy_attack"))
            {   
                //reset rotation
                transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,0f));
                spearAreaOfEffect.enabled = false;
            }
        }
        else{
             //reset rotation
                transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,0f));
                spearAreaOfEffect.enabled = false;
        }
    }

    private void RotateTowards(){
         //Get our Screen position
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (transform.position);
        //Get the Screen position of target
        Vector2 targetOnScreen = Camera.main.WorldToViewportPoint (target.transform.position);
         
        //Get the angle between the points
        float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        float angle = AngleBetweenTwoPoints(positionOnScreen, targetOnScreen);

        if(transform.lossyScale.x > 0){
            transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle-180));
        }
        else{
            transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle));
        }
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
            if(enemyState == EnemyState.Idle){
                //hit some obstacle while on Idle mode

                if(other.transform.position.x > transform.position.x){
                    nextDir = Vector2.left;
                }
                else{ nextDir = Vector2.right; }
            }

            if(enemyState == EnemyState.Attack){
                if(other.gameObject == target){    //some recoil when spear attack connects
                    rb.AddForce((transform.position - target.transform.position).normalized * attackDashForce);
                }
            }

            if(other.gameObject.layer == LayerMask.NameToLayer("projectiles")){
                //was hit
                Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
            
                rb.velocity = new Vector2(0,rb.velocity.y);
                isStunned = true;

                if(aggroWhenHit){ isAggro = true; }

                //reset timer
                nextTime = Time.time + attackDelay;
            }
        }
    }

    private void UpdatePath(){
        if(enemyState == EnemyState.Attack){
            if(seeker.IsDone()){
                seeker.StartPath(rb.position, target.transform.position, OnPathComplete);
            }
        }
    }

    private void OnPathComplete(Path p){
        if(!p.error){
            path = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate() {
        //RotateTowards();
        if(enemyState == EnemyState.Attack){
            FollowAiPath();
        }
    }

    private void FollowAiPath(){
        if(path == null){
            return;
        }

        if(currentWaypoint >= path.vectorPath.Count){
            reachedEndOfPath = true;
            return;
        }
        else{
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        rb.AddForce(direction * attackSpeed * Time.deltaTime);

        float distance = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);

        if(distance < nextWayPointDistance){
            currentWaypoint++;
        }
    }
}
