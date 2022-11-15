using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePartWhenFlip : MonoBehaviour
{
    [System.Serializable] 
    public class PartsAndParameters{
            public Transform part;
            public int numberSpriteLevels;
            public bool keepRelativePosition;
    }
    [System.Serializable] 
    public class Config{
        public PartsAndParameters[] partsAndParameters;

    }
    [SerializeField] public Config config;
    private float localScaleX;

    // Start is called before the first frame update
    void Start()
    {
        localScaleX = transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        if((config.partsAndParameters.Length != 0) && (transform.localScale.x != localScaleX)){
            HideOnFlip();
            localScaleX = transform.localScale.x;   //reset
        }
    }

    private void HideOnFlip(){

        Transform currPart;
        for(int i = 0; i < config.partsAndParameters.Length; i++){
            currPart = config.partsAndParameters[i].part; 

            //change sorting layer of hand's child sprites
            Transform currChild;
            for(int j = 0; j < currPart.childCount; j++){
                currChild = currPart.GetChild(j);

                if(currChild.TryGetComponent<SpriteRenderer>(out SpriteRenderer sp)){
                    if(transform.localScale.x < 0){
                        //sp.sortingLayerName = "player";
                        sp.sortingOrder = sp.sortingOrder + config.partsAndParameters[i].numberSpriteLevels;
                    }
                    else{
                        sp.sortingOrder = sp.sortingOrder - config.partsAndParameters[i].numberSpriteLevels;
                    }
                }
            }

            if(config.partsAndParameters[i].keepRelativePosition){
                //this keeps the part in the same relative position even when flipped
                currPart.localPosition = new Vector2(-currPart.localPosition.x, currPart.localPosition.y);
            }
        }
    }
}