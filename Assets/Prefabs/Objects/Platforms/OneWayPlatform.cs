using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private float turnBackOnDelay = 0.5f;
    
    private List<Collider2D> playerColliders;
    private bool onPlayer = false;
    private bool goingThrough = false;

    // Start i called before the first frame update
    void Start()
    {
        playerColliders = getAllColliders(player);
    }

    // Update is called once per frame
    void Update()
    {
        if(onPlayer){
            if(!goingThrough && Input.GetAxisRaw("Vertical") < 0){
                goingThrough = true;
            }
            if(goingThrough){
                goingThrough = false;
                StartCoroutine(waitToTurnBackOn(turnBackOnDelay));
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject == player){ onPlayer = true; }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject == player){ onPlayer = false; }
    }

    private IEnumerator waitToTurnBackOn(float seconds){
        foreach (Collider2D collider in playerColliders){
            Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), true);
        }
        yield return new WaitForSeconds(seconds);

        foreach (Collider2D collider in playerColliders){
            Physics2D.IgnoreCollision(collider, GetComponent<Collider2D>(), false);
        }
    }

    private List<Collider2D> getAllColliders(GameObject o){
        List<Collider2D> list = new List<Collider2D>();

        foreach(Collider2D collider in player.GetComponents<Collider2D>()){
            if(!list.Contains(collider)) { list.Add(collider); }
        }
        foreach(Collider2D collider in player.GetComponentsInChildren<Collider2D>()){
            if(!list.Contains(collider)) { list.Add(collider); }
        }

        return list;
    }
}
