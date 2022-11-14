using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipOnMovement : MonoBehaviour
{
    private float localScaleX;
    private float posX;

    // Start is called before the first frame update
    void Start()
    {
        localScaleX = transform.localScale.x;
        posX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.x != posX){
            if(transform.position.x > posX){
                if(localScaleX < 0){
                    transform.localScale = new Vector3(transform.localScale.x * -1,
                     transform.localScale.y, transform.localScale.z);
                    localScaleX = 1;
                }
            }
            else{
                if(localScaleX > 0){
                    transform.localScale = new Vector3(transform.localScale.x * -1,
                     transform.localScale.y, transform.localScale.z);
                    localScaleX = -1;
                }
            }
        }

        posX = transform.position.x;
    }
}
