using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnButtonPressDoAction : MonoBehaviour
{
    [System.Serializable]
    private class ButtonAction{
        public KeyCode button;
        public UnityEvent action;
    }

    [SerializeField] private ButtonAction[] actionList;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {



        for(int i = 0; i < actionList.Length; i++){
            if(Input.GetKeyDown(actionList[i].button)){
                actionList[i].action.Invoke();
            }
        }
    }
}
