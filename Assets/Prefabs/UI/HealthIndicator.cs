using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthIndicator : MonoBehaviour
{
   //[SerializeField] private string tagHealthToShow;
    [SerializeField] private float startHealth = 6;
    [SerializeField] private Sprite full;
    [SerializeField] private Sprite half;
    [SerializeField] private Sprite empty;

    private enum Status{
        FULL,
        HALF,
        EMPTY
    }

    [System.Serializable]
    private class HealthUnit{
        public Status status;
        public GameObject unit;
    }

    [SerializeField] private HealthUnit[] indicator = new HealthUnit[3];

    // Start is called before the first frame update
    void Start()
    {
        //idea for making a dynamic health bar that resizes for the amount of health we give it
        /*if(tagHealthToShow != null){
            Health[] obj = FindObjectsOfType<Health>();

            for(int i = 0; i < obj.Length; i++){
                if(obj[i].tag == tagHealthToShow){
                    currHealth = obj[i].Hp();
                }
            }
        }*/
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HealthDown(){
        startHealth -= 1;

        switch(startHealth){
            case 5:
                indicator[2].status = Status.HALF;
                indicator[2].unit.GetComponent<Image>().sprite = half;
                break;
            case 4:
                indicator[2].status = Status.EMPTY;
                indicator[2].unit.SetActive(false);
                break;
            case 3:
                indicator[1].status = Status.HALF;
                indicator[1].unit.GetComponent<Image>().sprite = half;
                break;
            case 2:
                indicator[1].status = Status.EMPTY;
                indicator[1].unit.SetActive(false);
                break;
            case 1:
                indicator[0].status = Status.HALF;
                indicator[0].unit.GetComponent<Image>().sprite = half;
                break;
            case 0:
                indicator[0].status = Status.EMPTY;
                indicator[0].unit.SetActive(false);
                break;
        }
    }
}
