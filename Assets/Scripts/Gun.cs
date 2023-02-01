using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] public GunObject gunObject;
    public string instrument;
    GameObject bulletPrefab;
    float bulletSpeed;
    float damage;
    public Player shooter;
    public bool soloActive = false;
    [SerializeField] List<GameObject> soloBullets;

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

    public void Shoot()
    {
        Debug.Log(this.name + " Shoot");
        if (shooter == null)
        {
            return;
        }
        
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        if (newBullet.GetComponent<Bullet>())
        {
            newBullet.GetComponent<Bullet>().shooter = shooter;
        }
        else
        {
            newBullet.GetComponent<Shotgun>().SetShooter(shooter);
        }
    }

    public void SoloShoot(int bulletInt)
    {
        GameObject newBullet = Instantiate(soloBullets[bulletInt], transform.position, transform.rotation);
        if (newBullet.GetComponent<Bullet>())
        {
            newBullet.GetComponent<Bullet>().shooter = shooter;
        }
        else
        {
            newBullet.GetComponent<Shotgun>().SetShooter(shooter);
        }
    }
}
