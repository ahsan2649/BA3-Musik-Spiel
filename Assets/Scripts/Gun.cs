using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] Transform bulletStart;
    [SerializeField] float bulletSpeed;
    [SerializeField] public string GunType;

    public void Fire()
    {
        if (bulletPrefab != null)
        {
            var bullet = Instantiate(bulletPrefab, bulletStart.position, Quaternion.identity);
            bullet.GetComponent<Bullet>().speed = bulletSpeed;
        }
    }
}
