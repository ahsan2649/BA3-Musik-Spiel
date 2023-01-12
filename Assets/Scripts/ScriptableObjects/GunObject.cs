using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Song", menuName = "ScriptableObject/Gun", order = 1)]

public class GunObject : ScriptableObject
{
    public Color color;
    public float bulletSpeed;
    public string instrument;
}
