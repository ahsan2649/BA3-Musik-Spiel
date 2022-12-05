using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    GameObject bulletPrefab;
    Transform bulletStart;
    float bulletSpeed;
    public string instrument;

    public void Start()
    {
        
    }

    public void Fire()
    {
        if (bulletPrefab != null)
        {
            var bullet = Instantiate(bulletPrefab, bulletStart.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().speed = bulletSpeed;
        }
    }

    public void SetInfo(float bulletSpeed, string GunType, GameObject bulletPrefab, Transform bulletStart)
    {
        this.bulletSpeed = bulletSpeed;
        this.instrument = GunType;
        this.bulletPrefab = bulletPrefab;
        this.bulletStart = bulletStart;

    }
}
