using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float m_hp = 100;
    [SerializeField] private float m_damageReductionPercent = 0;
    [SerializeField] private UnityEvent onHealthZero;

    public void Change(float amount){
        if(amount < 0){
             m_hp += amount - (amount * m_damageReductionPercent);
        } 
        else{
            m_hp += amount;
        }

        if(m_hp <= 0){
            onHealthZero.Invoke();
        }
    }


}
