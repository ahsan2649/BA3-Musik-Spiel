using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] float lifeTime = 1f;
    public Character shooter;

    Bullet[] bullets;
    private void Start()
    {
        //forwards shooter to each bullet
        bullets = GetComponentsInChildren<Bullet>();
        foreach (Bullet i in bullets)
        {
            i.shooter = shooter;
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Destroys bullet after some time
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}