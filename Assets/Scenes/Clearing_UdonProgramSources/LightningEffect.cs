
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class LightningEffect : UdonSharpBehaviour
{
    private float timer;
    public AudioSource[] audioSources;

    private void Start()
    {
        timer = 0; 
    }
    void Update()
    {
        
        timer += Time.deltaTime; 
        if(timer > 4)
        {
            timer = 0;
            int random = Random.Range(0, audioSources.Length);
            audioSources[random].Play(); 

        }
    }
}
