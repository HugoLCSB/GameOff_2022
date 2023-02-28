using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CannonHandler : MonoBehaviour
{
    [SerializeField] private CharacterMovement player;
    [SerializeField] private Transform firePoint;
    [SerializeField] private Light2D lightR;
    [SerializeField] private Light2D lightL;
    [SerializeField] private GameObject explosion;
    [SerializeField] private ParticleSystem smoke;


    private bool isOverHeating = false;

    private Color startColor;

    private Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        if(TryGetComponent<Animator>(out Animator a)){
            anim = a;
        }

        startColor = lightR.color;
        Debug.Log(startColor);
    }

    // Update is called once per frame
    void Update()
    {

        /*if(currHeatAmount <= 0 && !isOverHeating){
            DoOverHeat();
        }*/
    }

    public void DoShoot(){

        Debug.Log("shoot");
        lightR.color = new Color(1.000f, lightR.color.g - (startColor.g * (player.GetHeatUpRate() / player.GetTotalOverHeat())), 0f, 1f);

        lightL.color = lightR.color;

        if(anim != null){
            anim.SetTrigger("shoot");
        }

        CreateExplosion();
    }

    public void DoOverHeat(){
        if(!isOverHeating){
            Debug.Log("overheat");
            isOverHeating = true;
            if(anim != null){
                anim.SetTrigger("overHeat");
            }
            smoke.Play();
            lightR.color = Color.red;
            lightL.color = Color.red;
        }
        else{
            Debug.Log("cooledDown");
            isOverHeating = false;
            if(anim != null){
                anim.SetTrigger("idle");
            }
            smoke.Stop();
            lightR.color = startColor;
            lightL.color = startColor;
        }
    }

    public void CreateExplosion(){
        GameObject explosionInstane =  Instantiate(explosion, firePoint.position, transform.rotation);
        if(explosionInstane.TryGetComponent<Light2D>(out Light2D explosionLight)){
            float greenValue = (explosionLight.color.g - startColor.g) + ((lightR.color.g) - (startColor.g * (player.GetHeatUpRate()/player.GetTotalOverHeat())));
            if(greenValue < 0){
                greenValue = 0;
            }
            explosionLight.color = new Color(1.000f, greenValue, 0f, 1f);
        }
    }

    public void everyTick(){
        //on Overheat fully cooled down
        
        //Debug.Log("tick");

        if(player.GetCurrOverHeat() < player.GetTotalOverHeat()){
            if(!isOverHeating){
                lightR.color = new Color(1.000f, lightR.color.g + (startColor.g * (player.GetCoolDown()/player.GetTotalOverHeat())), 0f, 1f);
                lightL.color = lightR.color;
            }
        }
    }
}
