using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

public class GunManager : MonoBehaviour
{
    [SerializeField] List<Gun> Guns;

    private void Awake()
    {
        Guns = GetComponentsInChildren<Gun>().ToList();
    }

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.beat += ShootCallback;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public FMOD.RESULT ShootCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, System.IntPtr instancePtr, System.IntPtr parameterPtr)
    {
        var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));

        Debug.Log((string)parameter.name);

        foreach (var gun in Guns)
        {
            if(gun.GunType == (string)parameter.name) gun.Fire();
        }

        return FMOD.RESULT.OK;
    }
}
