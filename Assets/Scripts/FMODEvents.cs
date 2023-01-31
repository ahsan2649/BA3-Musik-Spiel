using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    
    [field: Header("MaxSpeed")]
    [field: SerializeField] public EventReference maxSpeed { get; private set; }

    [field: Header("Grinding")]
    [field: SerializeField] public EventReference grinding { get; private set; }
    
    [field: Header("Slide Dash")]
    [field: SerializeField] public EventReference slideDash { get; private set; }

    [field: Header("Completed Slide")]
    [field: SerializeField] public EventReference completeSlide { get; private set; }

    [field: Header("Jump")]
    [field: SerializeField] public EventReference jump { get; private set; }

    [field: Header("Hit")]
    [field: SerializeField] public EventReference hit { get; private set; }

    [field: Header("Weapon Pickup")]
    [field: SerializeField] public EventReference weaponPickup { get; private set; }

    [field: Header("Dash")]
    [field: SerializeField] public EventReference dash { get; private set; }

    [field: Header("Miss")]
    [field: SerializeField] public EventReference miss { get; private set; }

    [field: Header("Health Pickup")]
    [field: SerializeField] public EventReference healthPickup { get; private set; }

    [field: Header("Crown")]
    [field: SerializeField] public EventReference crown { get; private set; }

    [field: Header("Death")]
    [field: SerializeField] public EventReference death { get; private set; }

    [field: Header("Pickup Spawn")]
    [field: SerializeField] public EventReference pickupSpawn { get; private set; }

    [field: Header("SOLO Destroyed")]
    [field: SerializeField] public EventReference soloDestroyed { get; private set; }



    [field: Header("UI")]

    [field: Header("Select")]
    [field: SerializeField] public EventReference select { get; private set; }

    [field: Header("Back")]
    [field: SerializeField] public EventReference back { get; private set; }

    [field: Header("Move")]
    [field: SerializeField] public EventReference move { get; private set; }

    [field: Header("Start")]
    [field: SerializeField] public EventReference start { get; private set; }


    public static FMODEvents instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.LogError("Found more than one FMODEvents");
        }
        instance = this;
    }
}
