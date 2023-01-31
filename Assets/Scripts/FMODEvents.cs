using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("SOLO Destroyed")]
    [field: SerializeField] public EventReference soloDestroyed { get; private set; }

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
