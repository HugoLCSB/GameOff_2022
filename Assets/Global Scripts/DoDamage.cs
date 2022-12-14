using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private float attackKnockBack = 5;
    [SerializeField] private LayerMask intendedTarget;
    [SerializeField] private Collider2D colliderToUse;
    private bool isEnabled = true;

    private void OnCollisionEnter2D(Collision2D col) {
        if(colliderToUse != null && col.otherCollider == colliderToUse){
            ApplyDamage(col.gameObject, col.contacts[0].point);
        }
        else{
             ApplyDamage(col.gameObject, col.contacts[0].point);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D other) {
        ApplyDamage(other.gameObject, transform.position);
    }*/

    private void ApplyDamage(GameObject other, Vector2 point){
        //check if the other has a Health component
        if(isEnabled && other.TryGetComponent<Health>(out Health objectToHit)){
            //is it demageable and in our target layer
            if(objectToHit.IsDamageable() && ((1<<objectToHit.gameObject.layer) == intendedTarget)){
                if(attackKnockBack != 0){   //attack with KnockBack
                    objectToHit.ChangeHp(-damageAmount, 
                    attackKnockBack, point);
                }
                else{objectToHit.ChangeHp(-damageAmount);}  //just do the damage

                Debug.Log("HIT(" + damageAmount + ") : " + this.name + " >> " + other.name);
            }
        }
    }

    public void isOn(bool state){
        isEnabled = state;
    }
}
