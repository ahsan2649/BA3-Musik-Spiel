using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    [SerializeField] float lifeTime = 1f;
    public Player shooter;

    Bullet[] bullets;
    public void SetShooter(Player shooter)
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
