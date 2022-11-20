using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    //[SerializeField] private string deathLayerName;//for falling or spikes or things that trigger death
    [SerializeField] private float runSpeed = 30;   //speed of the player
    [SerializeField] private float jumpDelay = 0.4f;    //cool down time between jumps
    [SerializeField] private float cannonRecoil = 12;   //KockBack received by the player after shooting
    [SerializeField] private float cannonPower = 5; //Power of the cannon shots
    [SerializeField] private float fallDownForce = 5;   //Force applied when player clicks down, to reach the floor quicker
    [SerializeField] private float fireRate = 2.8f; //cannon fire rate
    [SerializeField] private float rateMultiplier = 0.3f;  //percentage of the fireRate to add each shot
    [SerializeField] private float rateCap = 15; //Maximum allowed fireRate 
    [SerializeField] private bool canJump = true;   //whether or not the player can jump

    public UnityEvent DeathEvent;
    private float horizontalMove;
    private float variableFireRate;
    private bool jump = false;
    private bool isJumping = false;
    private bool isShooting = false;
    //private bool grounded = false;
    private float localScaleX;
    private Rigidbody2D rb;
    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        variableFireRate = fireRate;
        localScaleX = transform.localScale.x;

        //Add a listener to the new Event. Calls action method when invoked
        /*controller.OnLandEvent.AddListener(Grounded);
        controller.OffLandEvent.AddListener(UnGrounded);*/

        //SetUp the death event
        if (DeathEvent == null)
            DeathEvent = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();

        if(transform.localScale.x != localScaleX){
            anim.SetTrigger("flip");
            localScaleX = transform.localScale.x;
        }
    }

    void FixedUpdate()
    {
        //sending movement info to the character controller
        if((horizontalMove != 0 || jump) && !isShooting){
            anim.SetBool("isMoving", true);
            anim.SetFloat("moveDir", horizontalMove);
            controller.Move(horizontalMove * runSpeed * Time.fixedDeltaTime, false, jump);
        }
        else{ anim.SetBool("isMoving", false); }
        jump = false;
    }

    /*void Grounded()
    {
        grounded = true;
        jump = false;
    }
    void UnGrounded() { grounded = false; }*/

    void InputManager()
    {
        //GET MOVEMENT DATA
        horizontalMove = Input.GetAxisRaw("Horizontal");

        //JUMP
        if(canJump){
            doJump();
        }

        //CHECK FIRE
        if(!isShooting){
           if(Input.GetButton("Fire1")){
                Shoot();
                StartCoroutine(waitShoot(1/variableFireRate));

                //adding to the fire rate progressively if the fire button is held
                if((variableFireRate > 0) && (variableFireRate < rateCap)){
                    variableFireRate += variableFireRate * rateMultiplier;
                }
            }
        }
        if(Input.GetButtonUp("Fire1") && variableFireRate != 0){
            variableFireRate = fireRate;    //reset fireRate
        }//Debug.Log(variableFireRate);

        //CLICK DOWN = FALLING FASTER
        if(Input.GetAxisRaw("Vertical") < 0 && fallDownForce > 0){
            rb.AddForce(Vector2.down * fallDownForce);
        }
    }

    private void doJump(){
        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            jump = true;
            StartCoroutine(waitJump(jumpDelay));
        }

        if (Input.GetButtonUp("Jump"))
        {
            jump = false;

            if(rb.velocity.y > 0){
                //fall down quicker when jump key not held
                rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
            }
        }
    }
    private IEnumerator waitJump(float time){
        isJumping = true;
        yield return new WaitForSeconds(time);
        isJumping = false;
    }

    private void Shoot(){
        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (transform.position);
        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //HANDLING RECOIL
        Vector2 recoilDir = (new Vector2(positionOnScreen.x - mouseOnScreen.x,      
            positionOnScreen.y - mouseOnScreen.y).normalized);

        //recoil with pure constant velocity
        //rb.velocity = (recoilDir * cannonRecoil);   //recommended recoil of around 12 and high

        //recoil with adding a force
        if((Mathf.Abs(recoilDir.x) < Mathf.Abs(recoilDir.y)) &&(recoilDir.y < 0)){
            rb.AddForce(recoilDir * cannonRecoil/1.5f, ForceMode2D.Impulse);
        }
        else if((Mathf.Abs(recoilDir.x) < Mathf.Abs(recoilDir.y)) && (recoilDir.y > 0)){
            rb.AddForce(recoilDir * cannonRecoil*1.5f, ForceMode2D.Impulse);
        }
        else{
           rb.AddForce(recoilDir * cannonRecoil, ForceMode2D.Impulse);
        }

        //creating bullets at the firePoint and adding velocity to them
        GameObject shotInstance =  Instantiate(bullet, firePoint.position, transform.rotation);
        shotInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(mouseOnScreen.x - positionOnScreen.x,
            mouseOnScreen.y - positionOnScreen.y).normalized * cannonPower);
    }
    private IEnumerator waitShoot(float time){
        isShooting = true;
        yield return new WaitForSeconds(time);
        isShooting = false;
    }
}
