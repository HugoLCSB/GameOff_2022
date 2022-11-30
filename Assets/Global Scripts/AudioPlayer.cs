using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioForAction[] audioList; 

    [System.Serializable]
    private class AudioForAction{
        public string actionName;
        public float cooldownTime;
        public AudioClip[] sounds;
        public AudioSource source;
    }

    private float nextTime = 0;
    private string lastSoundName;

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
        if(((name == lastSoundName) && (Time.time > nextTime)) || (!(name == lastSoundName))){
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
            lastSoundName = audioList[index].actionName;
        }
    }
}
