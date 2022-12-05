using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Song", menuName = "ScriptableObject/Song", order = 1)]
public class SongObject : ScriptableObject
{
    [Serializable]
    public class Instrument
    {
        public string name;
        public bool on;
        public GunObject GunObject;
    }

    [Serializable]
    public class Phase
    {
        public string name;
        public bool isLooping;
        public List<string> availableWeapons;
    }

    public EventReference Song;


    public List<Instrument> Instruments;
    public List<Phase> Phases;
}
