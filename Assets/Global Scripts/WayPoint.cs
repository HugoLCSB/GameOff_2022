using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] public class WayPoint : MonoBehaviour{
    public Transform point;
    public bool useX; public bool negativeDirX; //direction is positive by default
    public bool useY; public bool negativeDirY; //direction is positive by default
}
