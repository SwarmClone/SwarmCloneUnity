using System.Collections;
using System.Collections.Generic;
using Live2D.Cubism.Framework.Motion;
using UnityEngine;
using Live2D.Cubism.Core;
using UnityEngine.Rendering;

public class motioncont_test : MonoBehaviour
{
    private static motioncont_test _instance;
    public static motioncont_test instance => _instance;
    // Start is called before the first frame update
    public CubismMotionController motionController;
    public List<AnimationClip> clips;
    private string motion;
    private CubismModel model;
    private void Awake()
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
    public string Motion{
        get=>motion;
        set
        {
            motion = value;
            if (!motionController.IsPlayingAnimation())
            {
                Debug.Log(motion);
                switch(motion)
                {
                    // 根据后续的motion3表情文件更改
                    case "like":
                        MotionPlayer(clips[Random.Range(2,3)]);
                        break;
                    case "disgust":
                        MotionPlayer(clips[4]);
                        break;
                    case "anger":
                        MotionPlayer(clips[5]);
                        break;
                    case "happy":
                        MotionPlayer(clips[6]);
                        break;
                    case "sad":
                        MotionPlayer(clips[7]);
                        break;
                    case "neutral":
                        break;
                }
            }
            else
                return;
        }

    }
    private void Start()
    {
        motionController = GetComponent<CubismMotionController>();
        if(motionController != null)
            Debug.Log("获取控制器");
    }

    // Update is called once per frame
    public void MotionPlayer(AnimationClip clip)
    {
        if(motionController == null || clip == null)
            Debug.LogError("控制器或切片为空");
        motionController.PlayAnimation(clip,isLoop:false);
    }

}
