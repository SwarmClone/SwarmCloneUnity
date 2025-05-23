using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using TMPro;
using UnityEngine;

public class State
{
    public bool motioncontroller_IsActivate = false;
    public bool textcontroller_IsActivate = false;
    public bool audiocontroller_IsActivate = false;
}

public class Manager : MonoBehaviour
{
    private static Manager _instance;
    public static Manager instance => _instance;
    public tts_queue tts_Queue;
    public request_socket request_Socket;
    public danmaku_queue danmaku_Queue;
    public State state = new State();

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
    }
    private void Start()
    {
        StartCoroutine(Model_talking());
    }


    private void Update()
    {
        
    }

    IEnumerator Model_talking()
    {   
        while(true)
        {
            if (tts_Queue.PutTtsData(out ttsdata data))
            {
                motioncontroller.instance.Motion = data.emotion;
                StartCoroutine(textcontroller.instance.PutText(data));
                audiocontroller.instance.PlayAudio(data);
                while (state.motioncontroller_IsActivate ||
                       state.textcontroller_IsActivate ||
                       state.audiocontroller_IsActivate)
                {
                    yield return null; // 每帧检查一次条件
                }
                if(data.isend)
                    request_Socket.SendDAudioFinished();
                
            }
            yield return null;  
        }

    }

}
