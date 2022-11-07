using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    [SerializeField] private float lifeTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(waitBeforeDeath(lifeTime));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if((other.transform.tag != "bullet") && (other.transform.tag != "Player")){
            //Destroy(gameObject);
        }
    }

    private IEnumerator waitBeforeDeath(float time){
        yield return new WaitForSeconds(time);
        Destroy(gameObject);
    }
}
