using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private float smoothing = 0.1f;
    [SerializeField] private bool takeChildrenIntoAccount = true;

    [SerializeField] private Vector2 limits;
    [SerializeField] private Vector2 offset;
    private Vector3 sum;

    private void FixedUpdate()
    {
        if(takeChildrenIntoAccount){
            //Get midpoint
            Vector3 sum = Vector3.zero;
            for(int i = 0; i < container.childCount; i++)
            {
                Transform target = container.GetChild(i);
                sum += target.gameObject.transform.position;
            }
            sum = sum / (container.childCount + 1);
            //sum = new Vector2(sum.x + offset.x, sum.y + offset.y);//accounting for offset

            //Bias the camera
            //Vector3 biased = new Vector3(sum.x, sum.y, sum.z);
            //for (int i = 0; i < bearContainer.childCount; i++)
            //{
            //    Transform target = bearContainer.GetChild(i);
            //    Vector3 diff = target.gameObject.transform.position - sum;
            //    biased += diff;
            //}
        }
        else{
            //Get the goal accounting for offset
            sum = new Vector2(container.position.x + offset.x, container.position.y + offset.y);
        }

        //Keep it in the limits
        Vector3 limited = Vector3.zero;
        limited.x =  Mathf.Clamp(sum.x, -limits.x, limits.x);
        limited.y =  Mathf.Clamp(sum.y, -limits.y, limits.y);

        Vector3 goal = limited;

        transform.position = Vector3.Lerp(transform.position, goal, 1 - smoothing);
        transform.position = new Vector3(
            transform.position.x,
            transform.position.y,
            -10);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(Vector2.zero, limits * 2);
    }
}