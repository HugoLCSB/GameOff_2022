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


















































    /*[SerializeField] private float idleTime = 3;
    [SerializeField] private float idleWalkSpeed = 5;
    [SerializeField] private float attackSpeed = 10;
    [SerializeField] private float attackDelay = 3;
    [SerializeField] private float attackDistance = 3;
    [SerializeField] private float aggroDistance = 10;
    [SerializeField] private bool aggroOnBothSides = false;
    [SerializeField] private bool canDoDamageWhileStopped = true;
    [SerializeField] private bool immuneWhileAttacking = false;

    private EnemyState enemyState = EnemyState.Idle;
    private float nextTime = 0;
    private float dirX;
    private float dirY;
    //private Vector2 nextDir = Vector2.zero;
    private bool isAggro = false;
    private bool isStunned = false;
    private Rigidbody2D rb;
    private Health health;
    private FlipOnMovement flip;
    private GameObject target; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if(immuneWhileAttacking){
            health = GetComponent<Health>();
        }

        flip = GetComponent<FlipOnMovement>();
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
            dirX = 0; dirY = 0;
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

        Debug.Log("NOW_ON -> " + nextState);
        return nextState;
    }

    private void DoIdle(){
        if(Time.time > nextTime){
            ChooseDir(ref dirX);
            ChooseDir(ref dirY);

            //reset timer
            nextTime = Time.time + Random.Range(idleTime-(idleTime*0.5f), idleTime+(idleTime*0.5f));
        } 

        if(aggroOnBothSides){
            if(CheckAggro(Vector2.right)){ isAggro = true; }
            else{ isAggro = CheckAggro(Vector2.left); }
        }else{ isAggro = CheckAggro(new Vector2(dirX, dirY));}    //only on the side he's looking/ walking towards

        //move
        rb.velocity = new Vector2(dirX, dirY).normalized * idleWalkSpeed;
    }

    private void ChooseDir(ref float dir){   //randomly chooses what to do next, sets nextDir
            switch(Random.Range(1,3)){
                case 1:
                    if(dir == 0 || dir == 1){
                        //last time we were either in "stop" or "right"
                        dir = 1;
                        Debug.Log("right");
                    }
                    else{
                        dir = 0;
                        Debug.Log("stop");
                    }
                    break;
                case 2:
                    if(dir == 0 || dir == -1){
                        //last time we were either in "stop" or "left"
                        dir = -1;
                        Debug.Log("left");
                    }
                    else{
                        dir = 0;
                        Debug.Log("stop");
                    }
                    break;
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
        
    }

    private void DoStunned(){
        if(Time.time > nextTime){
            if(aggroOnBothSides){
                if(CheckAggro(Vector2.right)){ isAggro = true; }
                else{ isAggro = CheckAggro(Vector2.left); }
            }else{ isAggro = CheckAggro(new Vector2(dirX, dirY));}    //only on the side he's looking/ walking towards

            isStunned = false;
        }
    }

     private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.layer != LayerMask.NameToLayer("ground")){
            if(other.transform.position.x > transform.position.x){
                dirY = -1;
            }
            else{  dirX = 1; }

            if(enemyState == EnemyState.Attack){
                if(other.gameObject.layer == LayerMask.NameToLayer("projectiles")){
                    //hit some obstacle while on Attack mode
                    //Debug.Log(LayerMask.LayerToName(other.gameObject.layer));
                
                    //rb.velocity = new Vector2(0,rb.velocity.y);
                    isStunned = true;

                    //reset timer
                    nextTime = Time.time + attackDelay;
                }
            }
        }
    }

     public void Die(){
        Debug.Log("Player Killed -Laser-");
        Destroy(gameObject);
    }*/
}
