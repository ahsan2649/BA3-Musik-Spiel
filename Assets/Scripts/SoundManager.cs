using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    private void Awake()
    {
        if(instance != null)
        {
            UnityEngine.Debug.LogError("Found more than one SoundManager");
        }
        instance = this;
    }
    

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }
}
