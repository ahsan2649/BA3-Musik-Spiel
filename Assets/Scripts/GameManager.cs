using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    [Serializable]
    class TimelineInfo
    {
        public int CurrentMusicBar = 0;
        public int CurrentMusicBeat = 0;
        public float tempo;
        public string currentMarker;
        public string lastMarker;
        public List<Gun> guns;

        public void Shoot(string marker)
        {
            foreach (var gun in guns)
            {
                if (marker.Contains(gun.instrument))
                {
                    gun.Shoot();
                }
            }
        }
    }

    [SerializeField] FMODUnity.EventReference song;
    [SerializeField] TimelineInfo timelineInfo;
    GCHandle timelineHandle;

    List<Character> characters= new List<Character>();
    List<Gun> guns= new List<Gun>();

    FMODUnity.StudioEventEmitter emitter;
    FMOD.Studio.EVENT_CALLBACK shootCallback;
    FMOD.Studio.EventInstance songInstance;



    // Start is called before the first frame update
    void Start()
    {
        characters = FindObjectsOfType<Character>().ToList();
        guns = FindObjectsOfType<Gun>().ToList();
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        timelineInfo = new TimelineInfo();
        timelineInfo.guns = guns;
        emitter.EventReference = song;
        shootCallback = new FMOD.Studio.EVENT_CALLBACK(ShootCallback);
    }

    private void OnDestroy()
    {
        songInstance.setUserData(IntPtr.Zero);
        emitter.Stop();
        songInstance.release();
        timelineHandle.Free();
    }

    public void StartSong()
    {
        emitter.Play();
        songInstance = emitter.EventInstance;
        timelineHandle = GCHandle.Alloc(timelineInfo);
        songInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        songInstance.setCallback(shootCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartSong();
        }
    }

    #region BeatsAndMarkers
    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
    static FMOD.RESULT ShootCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

        IntPtr timelineInfoPtr;
        FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

        if (result != FMOD.RESULT.OK)
        {
            Debug.LogError("Timeline Callback error: " + result);
        }
        else if (timelineInfoPtr != IntPtr.Zero)
        {
            GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
            TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

            switch (type)
            {
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                    var markerParams = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                    timelineInfo.currentMarker = (string)markerParams.name;
                    timelineInfo.Shoot((string)markerParams.name);
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    var beatParams = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                    timelineInfo.CurrentMusicBar = beatParams.bar;
                    timelineInfo.CurrentMusicBeat = beatParams.beat;
                    timelineInfo.tempo = beatParams.tempo;
                    break;
            }
        }
       
        return FMOD.RESULT.OK;
    }
    #endregion
}
