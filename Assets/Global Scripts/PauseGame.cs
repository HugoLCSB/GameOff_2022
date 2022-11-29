using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PauseGame : MonoBehaviour
{
    [SerializeField] private float pauseTimeScale = 0;
    /*[SerializeField] private UnityEvent[] actionOnPause;
    [SerializeField] private UnityEvent[] actionOnUnpause;*/
    [SerializeField] private GameObject[] objectsToPause;

    private float regularTimeScale;

    private void Start() {
        regularTimeScale = Time.timeScale;
    }

    public void Pause(){
        Time.timeScale = pauseTimeScale;
        /*for(int i = 0; i < actionOnPause.Length; i++){
            actionOnPause[i].Invoke();
        }*/

        if(objectsToPause.Length != 0){
            for(int j = 0; j < objectsToPause.Length; j++){
                objectsToPause[j].SendMessage("PauseUnPause", true);
            }
        }
    }

    public void UnPause(){
        Time.timeScale = regularTimeScale;
        /*for(int i = 0; i < actionOnUnpause.Length; i++){
            actionOnUnpause[i].Invoke();
        }*/

        if(objectsToPause.Length != 0){
            for(int j = 0; j < objectsToPause.Length; j++){
                objectsToPause[j].SendMessage("PauseUnPause", false);
            }
        }
    }
}
