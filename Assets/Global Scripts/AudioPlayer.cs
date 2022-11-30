using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioForAction[] audioList; 

    [System.Serializable]
    private class AudioForAction{
        public AudioSource source;
        public string actionName;
        public float cooldownTime;
        public AudioClip[] sounds;
    }

    //private AudioSource source;
    private float nextTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        //source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlaySound(string name){
        if(Time.time > nextTime){
            for(int i = 0; i < audioList.Length; i++){
                if(audioList[i].actionName == name){
                    PlayRandom(i);
                    nextTime = Time.time + audioList[i].cooldownTime;
                }
            }
        }
    }

    private void PlayRandom(int index){
        if(audioList[index].sounds.Length > 0){
            int num = Random.Range(0, audioList[index].sounds.Length);
            audioList[index].source.clip = audioList[index].sounds[num];
            audioList[index].source.Play();
        }
    }
}
