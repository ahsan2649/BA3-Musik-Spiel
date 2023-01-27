using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShineRotator : MonoBehaviour
{
    [SerializeField] float animationSpeed;
    [SerializeField] float animationStartTime;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Animator>().SetFloat("animSpeed", animationSpeed);
        GetComponent<Animator>().Play("rotate_shine", 0, animationStartTime);
    }
}
