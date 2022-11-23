using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipOnMovement : MonoBehaviour
{
    [SerializeField] private Transform target;
    private float posX;

    // Update is called once per frame
    void Update()
    {
        if(target == null){
            if(transform.position.x != posX){
                Flip(transform.position.x); //flip on movement
            }
        }
        else{ Flip(target.position.x); }    //flip to target

        posX = transform.position.x;
    }

    private void Flip(float xValueToWatch){
        Vector3 newLocalScale = new Vector3(transform.localScale.x * -1,
                    transform.localScale.y, transform.localScale.z);

        if(xValueToWatch > posX){
            if(transform.localScale.x < 0){ //flip right
                transform.localScale = newLocalScale;
            }
        }
        else{
            if(transform.localScale.x > 0){ //flip left
                transform.localScale = newLocalScale;
            }
        }
    }

    public void SetTarget(Transform newTarget){
        target = newTarget;
    }
}
