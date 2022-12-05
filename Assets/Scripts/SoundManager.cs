using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] SongObject Song;
    [SerializeField] public List<SongObject.Instrument> Weapons;
    [SerializeField] public List<SongObject.Phase> Phases;
    [SerializeField] public List<SongObject.OptTrack> OptTracks;


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
        Weapons = Song.Instruments;
        Phases = Song.Phases;
        OptTracks = Song.OptTracks;

    }

    // Start is called before the first frame update
    void Start()
    {
        beat += new FMOD.Studio.EVENT_CALLBACK(GunManager.instance.ShootCallback);

        emitter.EventDescription.setCallback(beat, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    // Update is called once per frame
    void Update()
    {
        SetWeapons();
        SetPhases();
        SetOptTracks();

    }

    private void OnApplicationQuit()
    {
        if(emitter.IsPlaying()) emitter.Stop();
    }

    private void OnDisable()
    {
        if (emitter.IsPlaying()) emitter.Stop();
    }


    void SetPhases()
    {
        foreach (SongObject.Phase phase in Phases)
        {
            emitter.EventInstance.setParameterByName(phase.name, phase.isLooping ? 1 : 0);
        }
    }

    void SetWeapons()
    {
        foreach (SongObject.Instrument weapon in Weapons)
        {
            emitter.EventInstance.setParameterByName(weapon.name, weapon.isOn ? 1 : 0);
        }
    }

    void SetOptTracks()
    {
        foreach (SongObject.OptTrack optTrack in OptTracks)
        {
            emitter.EventInstance.setParameterByName(optTrack.name, optTrack.isOn ? 1 : 0);
        }
    }


}
