using UnityEngine;
using Live2D.Cubism.Framework.MouthMovement;
using Live2D.Cubism.Framework;

public class motioncontroller : MonoBehaviour
{
    public bool HasUpdateController { get; set; }

    public bool NeedsUpdateOnEditing
    {
        get { return false; }
    }

    public int ExecutionOrder
    {
        get { return 50; }
    }


    private static motioncontroller _instance;
    public static motioncontroller instance => _instance;
    private CubismMouthController mouthController;
    private CubismEyeBlinkController eyesController;

    private Animator anim;
    private string motion;
    public string Motion{
        get=>motion;
        set
        {
            motion = value;
            Debug.Log(motion);   
            if(anim.GetCurrentAnimatorStateInfo(0).IsTag("idel"))
            {
                Manager.instance.state.motioncontroller_IsActivate=true;
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
        anim = GetComponent<Animator>();
        eyesController = GetComponent<CubismEyeBlinkController>();
    }

    // Update is called once per frame
    public void Update()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsTag("idel"))
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
