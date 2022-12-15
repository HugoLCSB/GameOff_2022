using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnscaledTimeToShader : MonoBehaviour
{
    Renderer ren;

    // Start is called before the first frame update
    void Start()
    {
       ren = GetComponent<Renderer>();
       Debug.Log(ren.GetType());
    }

    // Update is called once per frame
    void Update()
    {
        ren.material.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}
