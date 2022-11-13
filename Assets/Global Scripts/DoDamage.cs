using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 1;
    [SerializeField] private float attackKnockBack = 5;

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.TryGetComponent<Health>(out Health objectToHit)){
            if(objectToHit.IsDamageable()){
                if(attackKnockBack != 0){   //attack with KnockBack
                    objectToHit.ChangeHp(-damageAmount, 
                    attackKnockBack, transform.position);
                }
                else{objectToHit.ChangeHp(-damageAmount);}  //just do the damage
            }
        }
    }
}
