using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAnimation : MonoBehaviour
{
    [SerializeField] private string triggerName;
    [SerializeField] private Animator anim;

    public void TriggerAnim(){
        if(triggerName != null && anim != null){
            anim.SetTrigger(triggerName);
        }
    }
}
