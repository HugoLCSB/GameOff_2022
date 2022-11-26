using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform container;
    [SerializeField] private float smoothing = 0.1f;
    [SerializeField] private bool takeChildrenIntoAccount = true;   //if you want to target more than one object
    [SerializeField] private Vector2 centerPoint;   //(x, y)
    [SerializeField] private Vector2 limitsX;   //(min, max)
    [SerializeField] private Vector2 limitsY;   //(min, max)
    [SerializeField] private Vector2 offset;    //(x, y)


    /*[System.Serializable] 
    public class WayPoint{
        public Transform point;
        public bool useX; public bool negativeDirX; //direction is positive by default
        public bool useY; public bool negativeDirY; //direction is positive by default

        /*public WayPoint(Transform nPoint, bool nUseX, bool nNegativeDirX,
                                                 bool nUseY, bool nNegativeDirY){
            this.point = nPoint;
            this.useX = nUseX;
            this.negativeDirX = nNegativeDirX;
            this.useY = nUseY;
            this.negativeDirY = nNegativeDirY;
        }
    }*/
    [SerializeField] private WayPoint[] stoppingAreas;    //points where the camera should stop


    private bool atWayPointX = false;
    private float lockPosXAt;
    private bool dirNegativeOnX = false;
    private bool atWayPointY = false;
    private float lockPosYAt;
    private bool dirNegativeOnY = false;

    private void FixedUpdate()
    {
        Vector3 limited = LimitMovement(GetPosToTarget());

        HandleWayPoints();

        transform.position = Vector3.Lerp(transform.position, limited, 1 - smoothing);
        transform.position = new Vector3(transform.position.x,transform.position.y,-10);
    }

    private Vector3 GetPosToTarget(){
        Vector3 sum = Vector3.zero;
        if(takeChildrenIntoAccount){
            //Get midpoint
            for(int i = 0; i < container.childCount; i++)
            {
                Transform target = container.GetChild(i);
                sum += target.gameObject.transform.position;
            }
            sum = sum / (container.childCount + 1);
        }
        else{
            //Get the goal accounting for offset
            sum = new Vector2(container.position.x + offset.x, container.position.y + offset.y);
        }
        return sum;
    }

    private Vector3 LimitMovement(Vector3 target){
        Vector3 limited = Vector3.zero;
        limited.x =  Mathf.Clamp(target.x, limitsX.x + centerPoint.x, limitsX.y + centerPoint.x);
        limited.y =  Mathf.Clamp(target.y, limitsY.x + centerPoint.y, limitsY.y + centerPoint.y);

        //dont move if target is beyond wayPoint
        if(atWayPointX){
            if((dirNegativeOnX && target.x <= lockPosXAt) || (!dirNegativeOnX && target.x >= lockPosXAt)){
                limited.x = lockPosXAt;
            }
        }
        if(atWayPointY){
            if((dirNegativeOnY && target.y <= lockPosYAt) || (!dirNegativeOnY && target.y >= lockPosYAt)){
                limited.y = lockPosYAt;
            }
        }
        return limited;
    }

    private void HandleWayPoints(){
        if(stoppingAreas.Length != 0){
            for(int i = 0; i < stoppingAreas.Length; i++){
                WayPoint currPoint = stoppingAreas[i];
                
                if(currPoint.useX){
                    if((currPoint.negativeDirX && transform.position.x <= currPoint.point.position.x) || 
                        (!currPoint.negativeDirX && transform.position.x >= currPoint.point.position.x)){
                            dirNegativeOnX = false;
                            atWayPointX = true;
                            lockPosXAt = currPoint.point.position.x;
                            if(currPoint.negativeDirX){ dirNegativeOnX = true; }
                    }
                    else{   //reset when waypoint changes
                        atWayPointX = false;
                        dirNegativeOnX = false;
                    }
                }
                else if(currPoint.useY){
                    if((currPoint.negativeDirY && transform.position.y <= currPoint.point.position.y) || 
                        (!currPoint.negativeDirY && transform.position.y >= currPoint.point.position.y)){
                            dirNegativeOnY = false;
                            atWayPointY = true;
                            lockPosYAt = currPoint.point.position.y;
                            if(currPoint.negativeDirY){ dirNegativeOnY = true; }
                    }
                    else{   //reset when waypoint changes
                        atWayPointY = false;
                        dirNegativeOnX = false;
                    }
                }
            }
        }
        else{
            atWayPointX = false;
            dirNegativeOnX = false;
            atWayPointY = false;
            dirNegativeOnX = false;
        }
    }

    public void RemoveWayPoint(int index){
        if(stoppingAreas.Length >= index+1){
            WayPoint[] newStoppingAreas = new WayPoint[stoppingAreas.Length-1];
            for(int i = 0; i < stoppingAreas.Length; i++){
                if(i < index){
                    newStoppingAreas[i] = stoppingAreas[i];
                }
                else if(i > index){
                    newStoppingAreas[i-1] = stoppingAreas[i];
                }
            }
            stoppingAreas = newStoppingAreas;
        }
    }

    public void SetNewWayPoint(WayPoint newPoint/*Transform point, bool useX, bool negativeDirX,
                                                 bool useY, bool negativeDirY*/){
        if(stoppingAreas != null){
            //WayPoint newPoint = new WayPoint(point, useX, negativeDirX, useY, negativeDirY);
            WayPoint[] newStoppingAreas = new WayPoint[stoppingAreas.Length+1];
            for(int i = 0; i < stoppingAreas.Length+1; i++){
                if(i < stoppingAreas.Length){
                    newStoppingAreas[i] = stoppingAreas[i];
                }
                else{
                    newStoppingAreas[i] = newPoint;
                }
            }
            stoppingAreas = newStoppingAreas;
        }
    }

    //Draw limits on the scene view
    private void OnDrawGizmosSelected()
    {   
        Vector2 lengthRectSides = new Vector2((limitsX.y - limitsX.x), (limitsY.y - limitsY.x));
        Vector2 rectCenter = new Vector2(centerPoint.x + (limitsX.y + limitsX.x)/2,
                                             centerPoint.y + (limitsY.y + limitsY.x)/2); 
                                                             
        Gizmos.DrawWireCube(rectCenter, lengthRectSides);
    }
}