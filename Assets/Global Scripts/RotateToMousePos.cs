using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMousePos : MonoBehaviour
{
    //[SerializeField] private Transform[] objectsToRotate;
    [SerializeField] private Vector2 rightAngleLimits = Vector2.zero;
    [SerializeField] private Vector2 leftAngleLimits = Vector2.zero;
    [SerializeField] private float mouseAngle;   //just to show mouse angle in editor

    private bool isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!isPaused){
            Rotate();
        }

        /*if(objectsToRotate.Length != 0){
            for(int i = 0; i < objectsToRotate.Length; i++){
                Rotate(objectsToRotate[i]);
            }
        }*/
    }

    private void Rotate(){
        //Get the Screen positions of the object
        Vector2 positionOnScreen = Camera.main.WorldToViewportPoint (transform.position);
        //Get the Screen position of the mouse
        Vector2 mouseOnScreen = (Vector2)Camera.main.ScreenToViewportPoint(Input.mousePosition);
         
        //Get the angle between the points
        float AngleBetweenTwoPoints(Vector3 a, Vector3 b) {
            return Mathf.Atan2(a.y - b.y, a.x - b.x) * Mathf.Rad2Deg;
        }

        /*float*/ mouseAngle = AngleBetweenTwoPoints(positionOnScreen, mouseOnScreen);

        CheckLimits(mouseAngle);
    }

    private void CheckLimits(float angle){
        if(transform.lossyScale.x > 0){ //RIGHT SIDE
            if(rightAngleLimits != Vector2.zero){   //with limits
                if(!IsBetween(angle, rightAngleLimits.x, rightAngleLimits.y)){
                    transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle-180));
                }
            }
            else{   //unlimited
                transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle-180));
            }
        }
        else{   //LEFT SIDE
            if(leftAngleLimits != Vector2.zero){    //with limits
                if(IsBetween(angle, leftAngleLimits.x, leftAngleLimits.y)){
                    transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle));
                }
            }
            else{   //unlimited
                transform.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle));
            }
        }
    }

    public bool IsBetween(double testValue, double bound1, double bound2)
    {
        if (bound1 > bound2)
            return testValue >= bound2 && testValue <= bound1;
        return testValue >= bound1 && testValue <= bound2;
    }

    /*private void Rotate(Transform objectToRotate){
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
        if(transform.localScale.x < 0){ //this deals with character flip
            objectToRotate.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle));
        }
        else{
            objectToRotate.rotation =  Quaternion.Euler (new Vector3(0f,0f,angle-180));
        }
    }*/

    public void PauseUnPause(bool b){
        isPaused = b;
    }
}
