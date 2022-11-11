using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float hp = 100;
    [SerializeField] private float damageReductionPercent = 0;
    [SerializeField] private bool isDamageable = true;
    [SerializeField] private UnityEvent onHealthZero;

    public void ChangeHp(float amount){
        if(amount < 0){
             hp += amount - (amount * damageReductionPercent);
        } 
        else{
            hp += amount;
        }

        if(hp <= 0){
            onHealthZero.Invoke();
        }
    }

    public bool IsDamageable(){
        return isDamageable;
    }
}
