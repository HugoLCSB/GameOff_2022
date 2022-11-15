using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateToMousePos : MonoBehaviour
{
    [SerializeField] private Transform[] objectsToRotate;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(objectsToRotate.Length != 0){
            for(int i = 0; i < objectsToRotate.Length; i++){
                Rotate(objectsToRotate[i]);
            }
        }
    }

    private void Rotate(Transform objectToRotate){
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
    }
}
