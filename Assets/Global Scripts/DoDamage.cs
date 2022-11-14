using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private float attackKnockBack = 5;
    [SerializeField] private LayerMask intendedTarget;
    private bool isEnabled = true;

    private void OnCollisionEnter2D(Collision2D other) {
        //check if the other has a Health component
        if(isEnabled && other.gameObject.TryGetComponent<Health>(out Health objectToHit)){
            //is it demageable and in our target layer
            if(objectToHit.IsDamageable() && ((1<<objectToHit.gameObject.layer) == intendedTarget)){
                if(attackKnockBack != 0){   //attack with KnockBack
                    objectToHit.ChangeHp(-damageAmount, 
                    attackKnockBack, transform.position);
                }
                else{objectToHit.ChangeHp(-damageAmount);}  //just do the damage

                Debug.Log("HIT(" + damageAmount + ") : " + this.name + " >> " + other.gameObject.name);
            }
        }
    }

    public void isOn(bool state){
        isEnabled = state;
    }
}
