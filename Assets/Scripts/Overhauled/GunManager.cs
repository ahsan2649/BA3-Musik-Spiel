using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class GunManager : MonoBehaviour
{

    public List<Gun> guns = new List<Gun>();
    [SerializeField] List<GunObject> gunObjects;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] GameObject weaponPrefab;

    void Start()
    {
        if (gunObjects.Count == 0) return;
        System.Random rnd = new System.Random();
        int n = gunObjects.Count;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            GunObject value = gunObjects[k];
            gunObjects[k] = gunObjects[n];
            gunObjects[n] = value;
        }

        for (int i = 0; i < PlayerPrefs.GetInt("PlayerCount"); i++)
        {
            var gun = Instantiate(weaponPrefab, spawnPoints[i].position, Quaternion.identity);
            gun.GetComponent<Gun>().gunObject = gunObjects[i];
            guns.Add(gun.GetComponent<Gun>());
        }
    }

    void Update()
    {

    }

    public void Shoot(string instrument)
    {
        if(guns.Count == 0) return;
        foreach (var gun in guns)
        {
            if (instrument.Contains(gun.instrument))
            {
                if (gun.soloActive)
                {
                    if (instrument.Contains("0")) { gun.SoloShoot(0); }
                    if (instrument.Contains("1")) { gun.SoloShoot(1); }
                    if (instrument.Contains("2")) { gun.SoloShoot(2); }
                    Debug.Log("SOLO SHOULD SHOOT CMON");
                }
                else if (instrument.Contains("Weapon4"))
                {
                    if (instrument.Contains("Laser"))
                    {
                        gun.Shoot();
                    }
                    else
                    {
                        gun.LaserBoltShoot();
                    }
                }
                else { gun.Shoot(); }
                
            }
        }
    }
}
