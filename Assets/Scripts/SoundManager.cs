using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] SongObject Song;
    [SerializeField] List<SongObject.Params> Weapons;
    [SerializeField] List<SongObject.Params> Phases;

    StudioEventEmitter emitter;

    public FMOD.Studio.EVENT_CALLBACK beat;

    private void Awake()
    {     
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }

        emitter = GetComponent<StudioEventEmitter>();

        emitter.EventReference = Song.Song;
        emitter.Preload = true;
        Weapons = Song.Weapons;
        Phases = Song.Phases;        
    }

    // Start is called before the first frame update
    void Start()
    {
        emitter.EventDescription.setCallback(beat, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    // Update is called once per frame
    void Update()
    {
        SetWeapons();
        SetPhases();
    }

    
    void SetPhases()
    {
        foreach (SongObject.Params phase in Phases)
        {
                emitter.EventInstance.setParameterByName(phase.name, phase.value ? 1 : 0);
        }
    }

    void SetWeapons()
    {
        foreach (SongObject.Params weapon in Weapons)
        {
            emitter.EventInstance.setParameterByName(weapon.name, weapon.value ? 1 : 0);
        }
    }

    
}
