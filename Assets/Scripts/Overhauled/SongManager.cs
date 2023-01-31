using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class SongManager : MonoBehaviour
{
    public static SongManager instance;

    [SerializeField] GameObject Light1;
    [SerializeField] GameObject Light2;

    [SerializeField] public Color Color11;
    [SerializeField] public Color Color12;
    [SerializeField] public Color Color21;
    [SerializeField] public Color Color22;

    

    [Serializable]
    public class TimelineInfo
    {
        public int CurrentMusicBeat = 0;
        public float tempo;
        public string currentMarker;
        public float beatInterval;
        public float timeUntilNextBeat;
        public float timeAfterPrevBeat;
        public GunManager gunManager;
        public bool firstcolor = true;
        public int soloTimePos;
        public bool soloActive= false;

        [SerializeField] public Color Color11;
        [SerializeField] public Color Color12;
        [SerializeField] public Color Color21;
        [SerializeField] public Color Color22;
        public SpriteRenderer sr1;
        public SpriteRenderer sr2;
        public void BeatEvent()
        {
            if (firstcolor)
            {
                if(sr1 != null) sr1.color = Color11;
                if(sr2 != null) sr2.color = Color21;
                firstcolor = !firstcolor;
                //Debug.Log(sr2.ToString());
            }
            else
            {
                if(sr1 != null) sr1.color = Color12;
                if(sr2 != null) sr2.color = Color22;
                firstcolor = !firstcolor;
            }
            Debug.Log("Beat");
        }
    }

    enum Beat
    {
        One_Three, Two_Four
    };
    [SerializeField] Beat KickBeat;

    [SerializeField] FMODUnity.EventReference Song;

    [SerializeField] public TimelineInfo timelineInfo;
    [SerializeField][Range(0, 1)] float KickMargin;
    float KickMarginValue;
    public bool canKick;

    FMODUnity.StudioEventEmitter emitter;
    FMOD.Studio.EVENT_CALLBACK shootCallback;
    FMOD.Studio.EventInstance songInstance;
    GCHandle timelineHandle;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        else
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.EventReference = Song;


        timelineInfo = new TimelineInfo();
        timelineInfo.gunManager = FindObjectOfType<GunManager>();

        if(Light1 != null)timelineInfo.sr1 = Light1.GetComponent<SpriteRenderer>();
        if(Light2 != null)timelineInfo.sr2 = Light2.GetComponent<SpriteRenderer>();
        timelineInfo.Color11 = Color11;
        timelineInfo.Color12 = Color12;
        timelineInfo.Color21 = Color21;
        timelineInfo.Color22 = Color22;

        timelineHandle = GCHandle.Alloc(timelineInfo);
        shootCallback = new FMOD.Studio.EVENT_CALLBACK(ShootCallback);

        StartSong();
        Debug.Log("Starting here");
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame)
        {
            StartSong();
        }
        if (LevelManager.levelManager.phase == LevelManager.Phase.Playing)
        {
            emitter.SetParameter("sequence_end", 1);
        }

        if (emitter.IsPlaying())
        {
            timelineInfo.timeAfterPrevBeat += Time.deltaTime;
            timelineInfo.timeUntilNextBeat -= Time.deltaTime;
        }

        CalculateKick();
    }

    private void OnDestroy()
    {
        StopSong();
    }

    public void CalculateKick()
    {
        KickMarginValue = Mathf.Lerp(0, timelineInfo.beatInterval, KickMargin);

        if (KickBeat == Beat.One_Three)
        {
            if ((timelineInfo.CurrentMusicBeat % 2 == 0 && timelineInfo.timeUntilNextBeat < KickMarginValue) ||
                (timelineInfo.CurrentMusicBeat % 2 != 0 && timelineInfo.timeAfterPrevBeat < KickMarginValue)) canKick = true;
            else canKick = false;
        }
        if (KickBeat == Beat.Two_Four)
        {
            if ((timelineInfo.CurrentMusicBeat % 2 != 0 && timelineInfo.timeUntilNextBeat < KickMarginValue) ||
                (timelineInfo.CurrentMusicBeat % 2 == 0 && timelineInfo.timeAfterPrevBeat < KickMarginValue)) canKick = true;
            else canKick = false;
        }
    }



    #region StartStopPauseResumeSong

    public void StartSong()
    {
        Debug.Log("Starting");
        if (emitter.IsPlaying())
        {
            StopSong();
        }
        emitter.Play();
        songInstance = emitter.EventInstance;
        songInstance.setUserData(GCHandle.ToIntPtr(timelineHandle));
        songInstance.setCallback(shootCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
    }

    public void StopSong()
    {
        songInstance.setUserData(IntPtr.Zero);
        emitter.Stop();
        songInstance.release();
        if (timelineHandle.IsAllocated) timelineHandle.Free();
    }

    public void PauseSong()
    {
        songInstance.setPaused(true);
    }
    public void ResumeSong()
    {
        songInstance.setPaused(false);
    }

    #endregion

    #region Beats and Markers
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
                    timelineInfo.gunManager.Shoot((string)markerParams.name);
                    timelineInfo.currentMarker = (string)markerParams.name;
                    if (markerParams.name == "End") LevelManager.levelManager.phase = LevelManager.Phase.Result;
                    if(markerParams.name == "Solo_End") 
                    { 
                        instance.setTimelinePosition(timelineInfo.soloTimePos);
                        timelineInfo.soloActive = false;
                    }
                    break;
                case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                    var beatParams = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                    timelineInfo.CurrentMusicBeat = beatParams.beat;
                    timelineInfo.tempo = beatParams.tempo;
                    timelineInfo.beatInterval = 60 / timelineInfo.tempo;
                    timelineInfo.timeUntilNextBeat = timelineInfo.beatInterval;
                    timelineInfo.timeAfterPrevBeat = 0;
                    if (beatParams.beat % 2 == 0) timelineInfo.BeatEvent();
                    break;
            }
        }

        return FMOD.RESULT.OK;
    }

    #endregion

    public void SetSolo(Solo solo)
    {
        while (timelineInfo.CurrentMusicBeat != 4 || timelineInfo.timeUntilNextBeat !<= 0.1f)
        {
            Debug.Log("Waiting for 4 Bar"); 
        }
        timelineInfo.soloActive = true;
        emitter.EventInstance.getTimelinePosition(out timelineInfo.soloTimePos);
        emitter.EventInstance.setParameterByName("solo_start", 1);
        solo.StopSoloSound();
    }
}
