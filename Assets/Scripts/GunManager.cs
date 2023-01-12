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
        if (instance != null && instance != this) Destroy(this);
        else instance = this;

        Guns = new List<Gun>();
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

        //SHOOT GUN
        Debug.Log(markerName);

        return FMOD.RESULT.OK;
    }
}
