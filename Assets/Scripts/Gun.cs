using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] GunObject gunObject;
    [HideInInspector] public string instrument;
    GameObject bulletPrefab;
    float bulletSpeed;
    float damage;
    public Character owner;

    private void Start()
    {
        instrument = gunObject.instrument;
        bulletSpeed = gunObject.bulletSpeed;
        damage = gunObject.damage;
        bulletPrefab= gunObject.bulletPrefab;
        if (GetComponent<SpriteRenderer>() != null)
        {
            GetComponent<SpriteRenderer>().color = gunObject.color;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shoot()
    {
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        newBullet.GetComponent<Bullet>().bulletSpeed = bulletSpeed;
        newBullet.GetComponent<Bullet>().shooter = owner;
        newBullet.GetComponent<Bullet>().damage = damage;
    }
}
