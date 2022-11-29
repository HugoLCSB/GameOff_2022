using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    [SerializeField] private float startWidth = 0.05f;
    [SerializeField] private float endWidth = 0.05f;
    [SerializeField] private Material material;
    [SerializeField] private Transform originPoint;
    [SerializeField] private Transform endPoint;
     
    // these are set in start
    private LineRenderer line;
     
    /*private Vector3 point01 = new Vector3(0,0,0);
    private Vector3 point02 = new Vector3(0,0,0);
    private Vector3 point03 = new Vector3(0,0,0);
    private Vector3 point04 = new Vector3(0,0,0);
    private Vector3 point05 = new Vector3(0,0,0);*/
     
     
     
    private void Start ()
    {
       line = this.gameObject.AddComponent<LineRenderer>();
       line.startWidth = startWidth;
       line.endWidth = endWidth;
       line.material = material;
       //line.positionCount = 5;
       //line.material = aMaterial;
       //line.renderer.enabled = true;
    }
     
    private void Update ()
    {   
        line.SetPosition(0, originPoint.position);
        line.SetPosition(1, endPoint.transform.position);
       /*line.SetPosition(0, point01);
       line.SetPosition(1, point02);
       line.SetPosition(2, point03);
       line.SetPosition(3, point04);
       line.SetPosition(4, point05);*/
    }

}
