using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidePartWhenFlip : MonoBehaviour
{
    [System.Serializable] 
    public class PartsAndParameters{
            public Transform part;
            public int numberSpriteLevels;
            public string newLayer;
            public bool keepRelativePosition;
            public bool enableSwitch;
    }
    [System.Serializable] 
    public class Config{
        public PartsAndParameters[] partsAndParameters;

    }
    [SerializeField] public Config config;
    

    /*[SerializeField] private Transform part;
    [SerializeField] private int numberSpriteLevels;
    [SerializeField] private bool keepRelativePosition;*/
    private float localScaleX;

    // Start is called before the first frame update
    void Start()
    {
        localScaleX = transform.lossyScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        /*if((config.partsAndParameters.Length != 0) && (transform.localScale.x != localScaleX)){
            HideOnFlip();
            localScaleX = transform.localScale.x;   //reset
        }*/
        if(((transform.lossyScale.x > 0) && (localScaleX < 0)) || (
            (transform.lossyScale.x < 0) && (localScaleX > 0))){
            HideOnFlip();
            localScaleX = transform.lossyScale.x;   //reset
        }
    }

    /*private void HideOnFlip(){

       //change sorting layer of hand's child sprites
        Transform currChild;
        for(int j = 0; j < transform.childCount; j++){
            currChild = transform.GetChild(j);

            if(currChild.TryGetComponent<SpriteRenderer>(out SpriteRenderer sp)){
                if(transform.lossyScale.x < 0){
                    sp.sortingOrder = sp.sortingOrder + numberSpriteLevels;
                }
                else{
                    sp.sortingOrder = sp.sortingOrder - numberSpriteLevels;
                }
            }
        }

        if(keepRelativePosition){
            //this keeps the part in the same relative position even when flipped
            transform.localPosition = new Vector2(-transform.localPosition.x, transform.localPosition.y);
        }
    }*/

    private void HideOnFlip(){

        PartsAndParameters curr;
        for(int i = 0; i < config.partsAndParameters.Length; i++){
            curr = config.partsAndParameters[i]; 

            if(curr.part.TryGetComponent<SpriteRenderer>(out SpriteRenderer sp)){
                if(curr.numberSpriteLevels != 0){
                    if(transform.lossyScale.x < 0){
                        sp.sortingOrder = sp.sortingOrder + curr.numberSpriteLevels;
                    }
                    else{
                        sp.sortingOrder = sp.sortingOrder - curr.numberSpriteLevels;
                    }
                }

                if(curr.newLayer != null){
                    string oldLayer = sp.sortingLayerName;
                    sp.sortingLayerID = SortingLayer.NameToID(curr.newLayer);
                    curr.newLayer = oldLayer;
                }
            }

            if(config.partsAndParameters[i].keepRelativePosition){
                //this keeps the part in the same relative position even when flipped
                curr.part.localPosition = new Vector2(-curr.part.localPosition.x, curr.part.localPosition.y);
            }

            if(config.partsAndParameters[i].enableSwitch){
                curr.part.gameObject.SetActive(!curr.part.gameObject.activeInHierarchy);
            }
        }
    }
}
