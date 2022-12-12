using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    /*[SerializeField] private GameObject explosion;
    [SerializeField] private GameObject arm;
    [SerializeField] private Light2D l1;
    [SerializeField] private Light2D l2;
    [SerializeField] private Color c1;
    [SerializeField] private Color c2;*/
    //[SerializeField] private string deathLayerName;//for falling or spikes or things that trigger death
    [SerializeField] private float runSpeed = 30;   //speed of the player
    [SerializeField] private float jumpDelay = 0.4f;    //cool down time between jumps
    [SerializeField] private float cannonRecoil = 12;   //KockBack received by the player after shooting
    [SerializeField] private float cannonPower = 5; //Power of the cannon shots
    [SerializeField] private float fallDownForce = 5;   //Force applied when player clicks down, to reach the floor quicker
    [SerializeField] private float fireRate = 2.8f; //cannon fire rate
    [SerializeField] private float rateMultiplier = 0.3f;  //percentage of the fireRate to add each shot
    [SerializeField] private float rateCap = 15; //Maximum allowed fireRate


    [SerializeField] private float overHeatAmount = 5;   //total overheating pool
    [SerializeField] private float heatUpRate = 2.8f; //every cannon shot increases it by this much 
    [SerializeField] private float coolDownAmount = 0.3f;  //every tick without shooting decreases it by this much
    [SerializeField] private float coolDownRate = 0.3f;  //time between cooldown ticks
    [SerializeField] private float coolDownMultiplier = 0.3f;


    [SerializeField] private bool canJump = false;   //whether or not the player can jump
    [SerializeField] private bool hasAirResistance = false;   //whether or not to apply air resistance along with the recoil

    [SerializeField] UnityEvent OnShoot;
    [SerializeField] UnityEvent OnHeatTick;
    [SerializeField] UnityEvent SwitchOverHeat;

    private float horizontalMove;
    private float variableFireRate;
    private bool jump = false;
    private bool isJumping = false;
    private bool isShooting = false;
    private bool isOverHeating = false;
    private float currHeatAmount = 0;
    private float lastTickTime = 0;
    private Coroutine currShootRoutine;
    private bool grounded = false;
    private float localScaleX;
    private int counter;
    private bool isGamePaused = false;
    private Rigidbody2D rb;
    private Animator anim;
    private AudioPlayer sound;

    // Start is called before the first frame update
    void Start()
    {
        variableFireRate = fireRate;
        counter = 0;
        currHeatAmount = overHeatAmount;
        localScaleX = transform.localScale.x;

        Debug.Log(currHeatAmount);

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if(TryGetComponent<AudioPlayer>(out AudioPlayer a)){
            sound = a;
        }

        //Add a listener to the new Event. Calls action method when invoked
        controller.OnLandEvent.AddListener(Grounded);
        controller.OffLandEvent.AddListener(UnGrounded);
    }

    // Update is called once per frame
    void Update()
    {
        if(!isGamePaused){
            InputManager();
        }

        if(transform.localScale.x != localScaleX){
            //anim.SetTrigger("flip");
            localScaleX = transform.localScale.x;
        }
        anim.SetFloat("localScale", localScaleX);
    }

    void FixedUpdate()
    {
        //sending movement info to the character controller
        if((horizontalMove != 0 || jump) && (!isShooting || isOverHeating)){
            anim.SetBool("isMoving", true);
            anim.SetFloat("moveDir", horizontalMove);

            if(grounded){
                PlaySound("Walk");
            }

            controller.Move(horizontalMove * runSpeed * Time.fixedDeltaTime, false, jump);
        }
        else{ anim.SetBool("isMoving", false); }
        jump = false;

        anim.SetFloat("movementY", rb.velocity.y);

        //overHeating
        if(currHeatAmount <= 0 && !isOverHeating){
            isOverHeating = true;
            SwitchOverHeat.Invoke();

            //overheat sound and particles (unity Event)

            currHeatAmount = 0;
            /*if(arm.TryGetComponent<Animator>(out Animator armAnim)){
                armAnim.SetTrigger("overHeat");
            }
            l1.color = c2;
            l2.color = c2;*/
        }

        if(Time.fixedTime > (lastTickTime + 1/coolDownRate)){
            lastTickTime = Time.fixedTime;
            OnHeatTick.Invoke();

            if(currHeatAmount < overHeatAmount){
                currHeatAmount += coolDownAmount;
                if(!isOverHeating){
                    //l1.color = new Color(1.000f, l1.color.g + (c1.g * (coolDownAmount/overHeatAmount)), 0f, 1f);
                    //l2.color = l1.color;
                }
                if(currHeatAmount >= overHeatAmount){ 
                    currHeatAmount = overHeatAmount;    //reset
                    if(isOverHeating){
                        isOverHeating = false;
                        SwitchOverHeat.Invoke();
                        /*if(arm.TryGetComponent<Animator>(out Animator armAnim)){
                            armAnim.SetTrigger("idle");
                        }
                        Light2D[] cannonLights = arm.GetComponentsInChildren<Light2D>();
                        if(cannonLights.Length > 0){
                            Debug.Log("got a light");
                            for(int i = 0; i < cannonLights.Length; i++){
                                Debug.Log(cannonLights[i].color);
                                cannonLights[i].color = Color.yellow;
                                Debug.Log(cannonLights[i].color);
                            }
                        }*/
                        //l1.color = c1;
                        //l2.color = c1;
                    }
                }
            }
        }
    }

    void Grounded()
    {
        grounded = true;
        jump = false;
        anim.SetBool("grounded", true);
        PlaySound("Landing");
    }
    void UnGrounded() { 
        grounded = false; 
        anim.SetBool("grounded", false);
    }

    void InputManager()
    {
        //GET MOVEMENT DATA
        horizontalMove = Input.GetAxisRaw("Horizontal");

        //JUMP
        if(canJump){
            doJump();
        }

        //CHECK FIRE

        if(Input.GetButtonDown("Fire1") && !isOverHeating){
            Shoot();
            isShooting = true;
            currShootRoutine = StartCoroutine(waitShoot(1/variableFireRate));
        }
        if(Input.GetButtonUp("Fire1")){
            StopCoroutine(currShootRoutine);
            isShooting = false;
            variableFireRate = fireRate;
            counter = 0;
        }

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
        //OverHeating
        currHeatAmount -= heatUpRate;
        /*l1.color = new Color(1.000f, l1.color.g - (c1.g * (heatUpRate/overHeatAmount)), 0f, 1f);
        l2.color = l1.color;
        Debug.Log(l1.color.g);
        if(arm.TryGetComponent<Animator>(out Animator armAnim)){
            armAnim.SetTrigger("shoot");
        }*/
        OnShoot.Invoke();
        anim.SetTrigger("shoot");
        PlaySound("Fire");

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

        if(hasAirResistance){
            //air resistance force
            Vector2 airResistance = -rb.velocity.normalized;
            airResistance.x *= Mathf.Pow(rb.velocity.x, 2)/2; airResistance.y *= Mathf.Pow(rb.velocity.y, 2)/2;
            rb.AddForce(airResistance);
        }

        //creating bullets at the firePoint and adding velocity to them
        /*GameObject explosionInstane =  Instantiate(explosion, firePoint.position, transform.rotation);
        if(explosionInstane.TryGetComponent<Light2D>(out Light2D explosionLight)){
            float greenValue = (explosionLight.color.g - c1.g) + ((l2.color.g) - (c1.g * (heatUpRate/overHeatAmount)));
            if(greenValue < 0){
                greenValue = 0;
            }
            explosionLight.color = new Color(1.000f, greenValue, 0f, 1f);
        }*/

        GameObject shotInstance =  Instantiate(bullet, firePoint.position, transform.rotation);
        shotInstance.GetComponent<Rigidbody2D>().AddForce(new Vector2(mouseOnScreen.x - positionOnScreen.x,
            mouseOnScreen.y - positionOnScreen.y).normalized * cannonPower);
    }

    private IEnumerator waitShoot(float time){
        yield return new WaitForSeconds(time);
        if(isShooting){
            if(!isOverHeating){
                Shoot();

                //adding to the fire rate progressively if the fire button is held
                if((variableFireRate > 0) && (variableFireRate < rateCap)){
                    variableFireRate += variableFireRate * rateMultiplier;
                }

                currShootRoutine = StartCoroutine(waitShoot(1/variableFireRate));
            }
        }
    }

    private void PlaySound(string soundName){
        if(sound != null){
            sound.PlaySound(soundName);
        }
        else{ Debug.Log("no AudioPlayer component is attacked"); }
    }

    public void onDamage(){
        anim.SetTrigger("damaged");
    }

    public void PauseUnPause(bool b){
        /*if(!isGamePaused){
            isGamePaused = true;
        }else{ isGamePaused = false; }*/

        isGamePaused = b;
    }

    public float GetTotalOverHeat(){
        return overHeatAmount;
    }

    public float GetCurrOverHeat(){
        return currHeatAmount;
    }

    public float GetCoolDown(){
        return coolDownAmount;
    }

    public float GetHeatUpRate(){
        return heatUpRate;
    }
}
