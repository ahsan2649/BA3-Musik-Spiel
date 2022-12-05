using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    public static GunManager instance;
    
    [SerializeField] GameObject gunPrefab;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] List<Gun> Guns;

    private void Awake()
    {
        if (instance != null && instance != this)Destroy(this);
        else instance = this;

        Guns = new List<Gun>();

        foreach (var weapon in SoundManager.instance.Weapons)
        {
            var gun = Instantiate(gunPrefab);
            gun.AddComponent<Gun>().SetInfo(weapon.GunObject.bulletSpeed, weapon.GunObject.name, bulletPrefab, gun.GetComponentInChildren<Transform>(), weapon.GunObject.color);
            Guns.Add(gun.GetComponent<Gun>());
            
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FMOD.RESULT ShootCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, System.IntPtr instancePtr, System.IntPtr parameterPtr)
    {
        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));

        string markerName = (string)parameter.name;
        foreach (var gun in Guns)
        {
            if (markerName.Contains(gun.instrument)) gun.Fire();
        }

        return FMOD.RESULT.OK;
    }
}
