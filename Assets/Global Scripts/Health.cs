using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private float hp = 100;
    [SerializeField] private float damageReductionPercent = 0;
    [SerializeField] private bool isDamageable = true;
    [SerializeField] private bool applyKnockBack = true;
    [SerializeField] private UnityEvent onHealthZero;

    public void ChangeHp(float healthAmount){
        SetHp(healthAmount);
    }

    public void ChangeHp(float healthAmount, float knockBack, Vector2 attackerPos){
        SetHp(healthAmount);
        if(applyKnockBack){ApplyKnockBack(knockBack, attackerPos);}
    }

    private void SetHp(float amount){
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

    private void ApplyKnockBack(float amount, Vector2 pos){
        if(TryGetComponent<Rigidbody2D>(out Rigidbody2D rb)){
            Vector2 dir = new Vector2(pos.x - transform.position.x, pos.y - transform.position.y).normalized;
            rb.AddForce(-dir * amount, ForceMode2D.Impulse);
        }
    }

    public bool IsDamageable(){
        return isDamageable;
    }

    public void Immunity(bool state){
        isDamageable = !state;
    }

    public float Hp(){
        return hp;
    }
}
