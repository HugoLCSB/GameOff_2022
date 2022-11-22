using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    [SerializeField] private Transform[] effects;
    [SerializeField] private float lifeTime = 0.5f;
    [SerializeField] private float ownVelocity = 0;
    [SerializeField] private LayerMask intendedTargets;
    private Vector2 dir;
    private Rigidbody2D rb;

    private void Awake() {
        if(effects.Length != 0){
            for(int i = 0; i < effects.Length; i++){
                Instantiate(effects[i], transform.position, transform.rotation);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(waitBeforeDeath(lifeTime));
    }

    // Update is called once per frame
    void Update()
    {
        if(ownVelocity != 0){
            rb.velocity = dir * ownVelocity;
        }
    }

    private void OnCollisionEnter2D(Collision2D other) {
        //if((1<<other.gameObject.layer) == intendedTargets){
        if(intendedTargets == (intendedTargets | (1 << other.gameObject.layer))){
            Destroy(gameObject);
            //GetComponent<Collider2D>().enabled = false;
        }
    }

    private IEnumerator waitBeforeDeath(float time){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }

    public void setDir(Vector2 newDir){
        this.dir = newDir;
    }
}
