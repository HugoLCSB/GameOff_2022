using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable] 
public class WayPoint : MonoBehaviour{
    public Transform target;
    public Transform point;
    public bool useX; public bool negativeDirX; //direction is positive by default
    public bool useY; public bool negativeDirY; //direction is positive by default

    public float deactivationDistance;
    public UnityEvent NextWayPoint;

    private bool done = false;

    private void Start() {
        
    }

    private void Update() {
        if(deactivationDistance != 0 && !done && CheckDistance()){
            NextWayPoint.Invoke();
            done = true;
        }

        if(!CheckDistance()){
            done = false;
        }
    }

    private bool CheckDistance(){
        if(useX && ((!negativeDirX && (target.position.x - point.position.x) >= deactivationDistance) ||
                    (negativeDirX && (target.position.x - point.position.x) <= -deactivationDistance))){
            return true;
        }
        else if(useY && ((!negativeDirY && (target.position.y - point.position.y) >= deactivationDistance) ||
                         (negativeDirY && (target.position.y - point.position.y) <= -deactivationDistance))){
            return true;
        }
        else{
            return false;
        }
    }

    //Draw limits on the scene view
    private void OnDrawGizmosSelected()
    {   
        Vector3 deactivationPoint = point.position;
        if(useX){
            if(!negativeDirX){
                deactivationPoint.x = deactivationPoint.x + deactivationDistance;
            }
            else{
                deactivationPoint.x = deactivationPoint.x - deactivationDistance;
            }
        }
        else if(useY){
            if(!negativeDirY){
                deactivationPoint.y = deactivationPoint.y + deactivationDistance;
            }
            else{
                deactivationPoint.y = deactivationPoint.y - deactivationDistance;
            }
        }

        Gizmos.DrawLine(point.position, deactivationPoint);
    }
}


