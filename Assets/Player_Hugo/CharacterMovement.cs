using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Transform hand;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bullet;
    [SerializeField] private string deathLayerName;
    [SerializeField] private float runSpeed = 30;   //speed of the player
    [SerializeField] private float jumpDelay = 0.4f;    //cool down time between jumps
    [SerializeField] private float cannonRecoil = 12;   //KockBack received by the player after shooting
    [SerializeField] private float cannonPower = 5; //Power of the cannon shots
    [SerializeField] private float fireCoolDown = 0.5f; //Cool down time between each shot
    [SerializeField] private float fallDownForce = 5;   //Force applied when player clicks down, to reach the floor quicker 
    [SerializeField] private bool canJump = true;   //whether or not the player can jump

    public UnityEvent DeathEvent;

    private float horizontalMove;
    private bool jump = false;
    private bool isJumping = false;
    private bool isShooting = false;
    private bool grounded = false;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //Add a listener to the new Event. Calls action method when invoked
        controller.OnLandEvent.AddListener(Grounded);
        controller.OffLandEvent.AddListener(UnGrounded);

        //SetUp the death event
        if (DeathEvent == null)
            DeathEvent = new UnityEvent();
    }

    // Update is called once per frame
    void Update()
    {
        InputManager();
        RotationToMousePos(hand);
    }

    void FixedUpdate()
    {
        if((horizontalMove != 0 || jump) && !isShooting){
            controller.Move(horizontalMove * runSpeed * Time.fixedDeltaTime, false, jump);
        }
        jump = false;
    }

    void Grounded()
    {
        //fxPlayer.PlayFx("land");
        grounded = true;
        jump = false;
    }
    void UnGrounded() { grounded = false; }

    void InputManager()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");

        if(canJump){
            doJump();
        }

        if(!isShooting){
           if(Input.GetButton("Fire1")){
                Shoot();
                StartCoroutine(waitShoot(fireCoolDown));
            }
        }

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

    private void RotationToMousePos(Transform objectToRotate){
        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (objectToRotate.position);
        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
         
        //Get the angle between the points
        float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
 
        //Ta Daaa
        if(transform.localScale.x < 0){
            objectToRotate.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle));
        }
        else{
            objectToRotate.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle-180));
        }
    }

    private void Shoot(){
        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (transform.position);
        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);

        //adding velocity according to the direction from mouse to player (kockback on player)
        rb.velocity = (new Vector2(positionOnScreen.x - mouseOnScreen.x,
            positionOnScreen.y - mouseOnScreen.y).normalized * cannonRecoil);

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
