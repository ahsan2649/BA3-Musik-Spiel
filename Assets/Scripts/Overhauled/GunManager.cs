using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class GunManager : MonoBehaviour
{

    List<Gun> guns = new List<Gun>();
    [SerializeField]List<GunObject> gunObjects;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] GameObject weaponPrefab;

    void Start()
    {
        for (int i = 0; i < gunObjects.Count; i++)
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
                gun.Shoot();
            }
        }
    }
}