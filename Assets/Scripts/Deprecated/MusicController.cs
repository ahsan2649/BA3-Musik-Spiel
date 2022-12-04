using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;
using FMOD.Studio;
using System;
using System.Runtime.InteropServices;
using Unity.VisualScripting;

public class MusicController : MonoBehaviour
{
    StudioEventEmitter emitter;

    [SerializeField][Range(0, 1)] int Bass;
    [SerializeField][Range(0, 1)] int Drums;
    [SerializeField][Range(0, 1)] int Lead;
    [SerializeField][Range(0, 1)] int Chords;

    int beat;

    EventInstance instance;
    EVENT_CALLBACK beatCallback;

    private void Awake()
    {
        emitter = GetComponent<StudioEventEmitter>();
        beatCallback = new EVENT_CALLBACK(BeatEventCallback);
    }

    // Start is called before the first frame update
    void Start()
    {
        emitter.EventInstance.setCallback(beatCallback, EVENT_CALLBACK_TYPE.TIMELINE_BEAT);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        SetParams();

    }

    private void SetParams()
    {
        emitter.SetParameter("Bass", Bass);
        emitter.SetParameter("Chords", Chords);
        emitter.SetParameter("Lead", Lead);
        emitter.SetParameter("Drums", Drums);
    }

    private void OnDestroy()
    {
        emitter.EventInstance.release();
        emitter.Stop();
    }

    private void OnGUI()
    {
        GUILayout.Box(String.Format("Current Beat = {0}", beat));
    }

    [AOT.MonoPInvokeCallback(typeof(EVENT_CALLBACK))]
    RESULT BeatEventCallback(EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
    {
        TIMELINE_BEAT_PROPERTIES beatProps = (TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(TIMELINE_BEAT_PROPERTIES));
        beat = beatProps.beat;
        return RESULT.OK;
    }
}
