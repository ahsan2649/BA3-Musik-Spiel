using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum Phase
    {
        Starting, Playing
    };

    enum Beat
    {
        One_Three, Two_Four
    };

    [Serializable]
    class TimelineInfo
    {
        public int CurrentMusicBar = 0;
        public int CurrentMusicBeat = 0;
        public float tempo;
        public string currentMarker;
        public string lastMarker;
        public float beatInterval;
        public float timeUntilNextBeat;
        public float timeAfterPrevBeat;
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

    public Phase phase = Phase.Starting;
    [SerializeField] Beat KickBeat;
    [SerializeField] FMODUnity.EventReference song;
    [SerializeField] TimelineInfo timelineInfo;
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField][Range(0,1)] float KickMargin;
    [SerializeField] GameObject playerPrefab;

    int playerCount = 0;
    List<Character> characters= new List<Character>();
    List<Gun> guns= new List<Gun>();
    float KickMarginValue;

    PlayerInputManager playerManager;

    FMODUnity.StudioEventEmitter emitter;
    FMOD.Studio.EVENT_CALLBACK shootCallback;
    FMOD.Studio.EventInstance songInstance;
    GCHandle timelineHandle;

    [HideInInspector] public bool canKick;


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
        emitter.EventReference = song;
        
        playerManager = GetComponent<PlayerInputManager>();
        
        playerManager.playerPrefab = playerPrefab;
        foreach (var gamePad in Gamepad.all)
        {
            Debug.Log(gamePad);
            playerManager.JoinPlayer(playerCount, -1, null, gamePad);
            playerCount++;
        }

        characters = FindObjectsOfType<Character>().ToList();
        guns = FindObjectsOfType<Gun>().ToList();
        
        timelineInfo = new TimelineInfo();
        timelineInfo.guns = guns;
        timelineHandle = GCHandle.Alloc(timelineInfo);
        shootCallback = new FMOD.Studio.EVENT_CALLBACK(ShootCallback);
    }

    // Update is called once per frame
    void Update()
    {
        if (!emitter.IsPlaying())
        {
            StartSong();
        }

        if (phase==Phase.Starting)
        {
            if (characters.All(character => character.RotationReady && character.kick) && canKick) phase = Phase.Playing;
        }
        timelineInfo.timeAfterPrevBeat += Time.deltaTime;
        timelineInfo.timeUntilNextBeat -= Time.deltaTime;


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

    public void StartSong()
    {
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
        if (timelineHandle != null) timelineHandle.Free();
    }

    void OnPlayerJoined(PlayerInput playerInput)
    {
        playerInput.GetComponent<Character>().spawnPoint = spawnPoints[playerInput.playerIndex];
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
                    timelineInfo.beatInterval = 60 / timelineInfo.tempo;
                    timelineInfo.timeUntilNextBeat = timelineInfo.beatInterval;
                    timelineInfo.timeAfterPrevBeat = 0;
                    break;
            }
        }
       
        return FMOD.RESULT.OK;
    }
    #endregion
}
