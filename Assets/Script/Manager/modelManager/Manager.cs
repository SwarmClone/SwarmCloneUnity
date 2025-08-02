using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using TMPro;
using UnityEngine;
using Live2D.Cubism.Core;
using Unity.Mathematics;
using System;

public enum modelstatu
{
    talking = 1,
    singing = 2,
    changing = 3, 
}
public class State
{
    public bool motioncontroller_IsActivate = false;
    public bool textcontroller_IsActivate = false;
    public bool audiocontroller_IsActivate = false;
    public bool MeshObjectController_IsChang = false;
    public modelstatu modelstatu;
}

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager instance => _instance;

    public tts_queue tts_Queue;
    public sing_queue sing_Queue;
    public danmaku_queue danmaku_Queue;
    public request_socket request_Socket;
    public GameObject audiovisbleupanel;
    public State state = new State();
    public modelstatu startstatu;

    public Vector3 songnameposition;
    public float[] backwidths;

    private modelstatu laststatu;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        state.modelstatu = startstatu;
        laststatu = startstatu;
    }
    private void Start()
    {
        
    }


    private void Update()
    {
        if (laststatu != state.modelstatu || state.modelstatu == modelstatu.changing )
        {
            changestate(state.modelstatu);
        }
        if (state.modelstatu == modelstatu.talking)
        {
            Model_talking();
        }
        else if (state.modelstatu == modelstatu.singing)
        {
            Model_singing();
        }
        
    }

    private void changestate(modelstatu modelstatu)
    {
        switch (modelstatu)
        {
            case modelstatu.talking:
                laststatu = state.modelstatu;
                state.modelstatu = modelstatu.changing;
                break;
            case modelstatu.singing:
                StartCoroutine(MeshObjectController.instance.singback());
                laststatu = state.modelstatu;
                state.modelstatu = modelstatu.changing;
                break;
            case modelstatu.changing:
                if (laststatu == modelstatu.talking && !state.MeshObjectController_IsChang)
                    state.modelstatu = laststatu;
                else if (laststatu == modelstatu.singing && !(state.motioncontroller_IsActivate || state.textcontroller_IsActivate || state.audiocontroller_IsActivate))
                {
                    state.modelstatu = laststatu;
                }
                break;
        }   
    }

    private void Model_talking()
    {
        if (state.motioncontroller_IsActivate || state.textcontroller_IsActivate || state.audiocontroller_IsActivate)
            return;
        if (tts_Queue.PutTtsData(out ttsdata data))
        {
            motioncontroller.instance.Motion = data.emotion;
            StartCoroutine(textcontroller.instance.PutText(data));
            audiocontroller.instance.PlayAudio(data);
            if (data.isend)
                request_Socket.SendDAudioFinished();
        }
    }

    private void Model_singing()
    {
        if (sing_Queue.PutSingData(out singdata data))
        {
            StartCoroutine(windowcontroller.instance.SongName(data, songnameposition,(data) =>{
                    StartCoroutine(textcontroller.instance.Putsubtitle(data));
                    audiocontroller.instance.PlayAudio(data);
                }));
        }
        return;   
    }
}
