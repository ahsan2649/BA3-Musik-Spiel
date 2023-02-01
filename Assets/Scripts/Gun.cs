using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Gun : MonoBehaviour
{
    [SerializeField] public GunObject gunObject;
    public string instrument;
    GameObject bulletPrefab;
    float bulletSpeed;
    float damage;
    public Player shooter;
    public bool soloActive = false;
    public float soloRainbowHueSpeed = 1f;
    [SerializeField] List<GameObject> soloBullets;

    private float _hueShiftSpeed = 0.2f;
    private float _saturation = 1f;
    private float _value = 1f;

    int laserCount = 0;
    private void Start()
    {
        instrument = gunObject.instrument;
        bulletSpeed = gunObject.bulletSpeed;
        damage = gunObject.damage;
        bulletPrefab= gunObject.bulletPrefab;
        if (GetComponentInChildren<SpriteRenderer>() != null)
        {
            GetComponentInChildren<SpriteRenderer>().material = gunObject.gunColorShader;
        }
    }

    private void Update()
    {
        
        if (soloActive)
        {
            float amountToShift = _hueShiftSpeed * Time.deltaTime;
            Color newColor = ShiftHueBy(GetComponentInChildren<SpriteRenderer>().material.GetColor("_Color"), amountToShift);
            GetComponentInChildren<SpriteRenderer>().material.SetColor("_Color", newColor);
        }
    }

    private Color ShiftHueBy(Color color, float amount)
    {
        // convert from RGB to HSV
        Color.RGBToHSV(color, out float hue, out float sat, out float val);

        // shift hue by amount
        hue += amount;
        sat = _saturation;
        val = _value;

        // convert back to RGB and return the color
        return Color.HSVToRGB(hue, sat, val);
    }

    public void Shoot()
    {
        GameObject newBullet;
        Debug.Log(this.name + " Shoot");
        if (shooter == null)
        {
            return;
        }
        
        if (bulletPrefab.GetComponent<Bullet>().laser)
        {
            if (laserCount == 0)
            {
                newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
                if (newBullet.GetComponent<Bullet>())
                {
                    newBullet.GetComponent<Bullet>().shooter = shooter;
                }
            }
            else
            {
                newBullet = Instantiate(gunObject.laserSecondary, transform.position, transform.rotation);
                if (newBullet.GetComponent<Bullet>())
                {
                    newBullet.GetComponent<Bullet>().shooter = shooter;
                }
                laserCount++;
                if (laserCount == 2)
                {
                    laserCount = 0;
                }
            }
        }
        else
        {
            newBullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
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
