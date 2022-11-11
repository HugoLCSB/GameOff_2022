using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoDamage : MonoBehaviour
{
    [SerializeField] private float damageAmount = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if(other.gameObject.TryGetComponent<Health>(out Health objectToHit)){
            if(objectToHit.IsDamageable()){
                objectToHit.ChangeHp(-damageAmount);
            }
        }
    }
}
