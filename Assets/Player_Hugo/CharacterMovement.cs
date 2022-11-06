using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] private CharacterController2D controller;
    [SerializeField] private Transform hand;
    [SerializeField] private string deathLayerName;
    [SerializeField] private float runSpeed = 30;
    [SerializeField] private float jumpDelay = 0.5f;

    public UnityEvent DeathEvent;

    private float horizontalMove;
    private bool jump = false;
    private bool isJumping = false;
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
        controller.Move(horizontalMove * runSpeed * Time.fixedDeltaTime, false, jump);
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

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            jump = true;
            StartCoroutine(waitNextJump(jumpDelay));
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

    private IEnumerator waitNextJump(float time){
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
        float angle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);
 
        //Ta Daaa
        if(transform.localScale.x < 0){
            objectToRotate.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle));
        }
        else{
            objectToRotate.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle-180));
        }
 
        float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }
    }
}
