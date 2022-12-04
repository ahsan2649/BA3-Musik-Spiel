using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Song", menuName = "ScriptableObject/Song", order = 1)]
public class SongObject : ScriptableObject
{
    public EventReference Song;

    [Serializable]
    public class Params
    {
        public string name;
        public bool value;
    }

    public List<Params> Weapons;
    public List<Params> Phases;
}
