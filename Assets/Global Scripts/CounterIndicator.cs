using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CounterIndicator : MonoBehaviour
{
   //[SerializeField] private string tagHealthToShow;
    [SerializeField] private float startNum = 0;
    [SerializeField] private GameObject num0;
    [SerializeField] private GameObject num1;
    [SerializeField] private GameObject num2;
    [SerializeField] private GameObject num3;
    [SerializeField] private UnityEvent onChange;
    [SerializeField] private UnityEvent onDone;

    // Start is called before the first frame update
    void Start()
    {
        num0.SetActive(true);
         num1.SetActive(false);
          num2.SetActive(false);
           num3.SetActive(false);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void enemyDown(){
        startNum += 1;
        onChange.Invoke();

        switch(startNum){
            case 3:
                num2.SetActive(false);
                num3.SetActive(true);
                break;
            case 2:
                num1.SetActive(false);
                num2.SetActive(true);
                break;
            case 1:
                num0.SetActive(false);
                num1.SetActive(true);
                break;
        }

        if(startNum == 3){
             onDone.Invoke();
        }
    }
}
