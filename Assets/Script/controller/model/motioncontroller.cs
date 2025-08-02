using UnityEngine;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Framework.Expression;
using System.Collections.Generic;
using Unity.VisualScripting;

public class motioncontroller : MonoBehaviour
{
    public List<int> emotionduration;

    private static motioncontroller _instance;
    public static motioncontroller instance => _instance;

    private CubismExpressionController expressionController;
    private int framecount = 0;
    private CubismEyeBlinkController eyesController;
    private Animator anim;


    private string motion;
    public string Motion
    {
        get => motion;
        set
        {
            motion = value;
            if (expressionController.enabled)
                expression3(motion);
            else
                motion3(motion);
        }
    }

    // Start is called before the first frame update
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
    void Start()
    {
        expressionController = GetComponent<CubismExpressionController>();
        anim = GetComponent<Animator>();
        eyesController = GetComponent<CubismEyeBlinkController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (expressionController.enabled)
        {
            if (framecount > 0)
                framecount--;
            else if (expressionController.CurrentExpressionIndex != 0)
                expressionController.CurrentExpressionIndex = 0;
        }
        if (expressionController.enabled)
        {
            if (expressionController.CurrentExpressionIndex == 0)
            {
                eyesController.enabled = true;
                Manager.instance.state.motioncontroller_IsActivate = false;
            }
            else
            {
                eyesController.EyeOpening = 1.0f;
                eyesController.enabled = false;
            }
        }
        else
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("idel"))
            {
                eyesController.enabled = true;
                Manager.instance.state.motioncontroller_IsActivate = false;
            }
            else
            {
                eyesController.EyeOpening = 1.0f;
                eyesController.enabled = false;
            } 
        }
    }

    private void expression3(string motion)
    {
        if (expressionController.CurrentExpressionIndex == 0)
        {
            Manager.instance.state.motioncontroller_IsActivate = true;
            switch (motion)
            {
                case "like":
                    expressionController.CurrentExpressionIndex = 1;
                    framecount += emotionduration[0];
                    break;
                case "disgust":
                    expressionController.CurrentExpressionIndex = 2;
                    framecount += emotionduration[1];
                    break;
                case "anger":
                    expressionController.CurrentExpressionIndex = 3;
                    framecount += emotionduration[2];
                    break;
                case "happy":
                    expressionController.CurrentExpressionIndex = 4;
                    framecount += emotionduration[3];
                    break;
                case "sad":
                    expressionController.CurrentExpressionIndex = 5;
                    framecount += emotionduration[4];
                    break;
                case "neutral":
                    break;
            }
        }
    }

    private void motion3(string motion)
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("idel"))
            {
                var random =Random.Range(1,2);
                switch(motion)
                {
                    case "like":
                        anim.SetTrigger("like"+random.ToString());
                        break;
                    case "disgust":
                        anim.SetTrigger("disgust");
                        break;
                    case "anger":
                        anim.SetTrigger("anger");
                        break;
                    case "happy":
                        anim.SetTrigger("happy"+random.ToString());
                        break;
                    case "sad":
                        anim.SetTrigger("sad"+random.ToString());
                        break;
                    case "neutral":
                        break;
                }
            }
    }

}
