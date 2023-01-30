using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Gun : MonoBehaviour
{
    [SerializeField] public GunObject gunObject;
    [HideInInspector] public string instrument;
    GameObject bulletPrefab;
    float bulletSpeed;
    float damage;
    public Player shooter;
    public bool soloActive = false;
    [SerializeField] List<GameObject> soloBullets;
    private int soloBulletInt= 0;

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
        if (shooter == null)
        {
            return;
        }
        if (soloActive)
        {
            SoloShoot();
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

    private void SoloShoot()
    {
        GameObject newBullet = Instantiate(soloBullets[soloBulletInt], transform.position, transform.rotation);
        if (newBullet.GetComponent<Bullet>())
        {
            newBullet.GetComponent<Bullet>().shooter = shooter;
        }
        else
        {
            newBullet.GetComponent<Shotgun>().SetShooter(shooter);
        }
        if(soloBulletInt < soloBullets.Count) { soloBulletInt++; }
        else { soloBulletInt = 0; }
    }
}
