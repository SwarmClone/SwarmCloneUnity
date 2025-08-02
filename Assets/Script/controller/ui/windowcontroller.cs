using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;
using System;
using System.Collections.Generic;

public class window
{
    public GameObject windowobject;
    public Material material;
    public Action<Material, float> setaphla;

    public window(GameObject windowobject, Material material, Action<Material, float> setaphla)
    {
        this.windowobject = windowobject;
        this.setaphla = setaphla;
        this.material = material;
    }
}

public class WaitIEnumerator : CustomYieldInstruction
    {
        private List<IEnumerator> enumerators;
        private bool condition;

        public WaitIEnumerator(List<IEnumerator> enumerators)
        {
            this.enumerators = enumerators;
        }

        public override bool keepWaiting
        {
            get
            {
                condition = false;
                foreach (var enumerator in enumerators)
                {
                    if (!(enumerator == null))
                        condition = true;

                }
                return condition;
            }
        }
    }

public class windowcontroller : MonoBehaviour
{
    private static windowcontroller _instance;
    public static windowcontroller instance => _instance;

    public GameObject textglitchwindow;
    public GameObject scalingwindow;
    public List<RectTransform> UI;

    
    private char[] randomChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()".ToCharArray();
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
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator SongName(singdata data, Vector3 songnameposition, Action<singdata> action)
    {
        List<IEnumerator> runnings = new List<IEnumerator>();
        List<window> singnamewindow = new List<window>();
        var runningsid = 0;
        runnings.Add(CreatScalingWindow(singnamewindow,()=>{ runnings[0] = null; }));
        runnings.Add(CreatGlitchWindow(data, singnamewindow,()=>{ runnings[1] = null;}));
        for(; runningsid < runnings.Count; runningsid++)
            StartCoroutine(runnings[runningsid]);
        yield return new WaitIEnumerator(runnings);
        runnings.Add(DestroyWindows(singnamewindow,()=> { runnings[2] = null; }));
        for(; runningsid < runnings.Count; runningsid++)
            StartCoroutine(runnings[runningsid]);
        yield return new WaitIEnumerator(runnings);
        action?.Invoke(data);
    }
    private IEnumerator CreatGlitchWindow<T>(T data, List<window> windows, Action action) where T : singdata
    {
        GameObject new_window = Instantiate(textglitchwindow);
        new_window.transform.SetParent(UI[0]);
        new_window.transform.SetLocalPositionAndRotation(new Vector3(-560,-20), new Quaternion(0, 0, 0, 0));
        var textComponent = new_window.GetComponent<TMP_Text>();
        var material = textComponent.material;
        windows.Add(new window(new_window, material, SetTextWindowAlpha));
        for(float t = 0.02f ;t <= 1;t += 0.02f)
        {
            SetBcakWindowAlpha(material, t);
            new_window.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            textComponent.text = GenerateRandomString(data.songname.Length, 0, data.songname);
            yield return null;
        }
        for (int i = 0; i < 200; i++)
        {
            textComponent.text = GenerateRandomString(data.songname.Length, i / (200 / (float)data.songname.Length), data.songname);
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        action?.Invoke();
    }

    private IEnumerator CreatScalingWindow(List<window> windows, Action action)
    {
        GameObject new_window = Instantiate(scalingwindow);
        new_window.transform.SetParent(UI[1]);
        new_window.transform.SetLocalPositionAndRotation(new Vector3(-14,-0.5f),Quaternion.identity);
        var material = new_window.transform.GetComponent<Image>().material;
        windows.Add(new window(new_window, material, SetBcakWindowAlpha));
        for (float t = 0.02f; t <= 1; t += 0.02f)
        {
            SetBcakWindowAlpha(material, t);
            new_window.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        action.Invoke();
    }
    private IEnumerator DestroyWindows(List<window> windows, Action action)
    {
        for (float t = 1f; t >= 0; t -= 0.02f)
        {
            foreach (var window in windows)
            {
                window.setaphla(window.material, t);
                window.windowobject.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            }
            yield return null;
        }
        foreach (var window in windows)
        {
            Destroy(window.windowobject);
        }
        action.Invoke();
    }

    private string GenerateRandomString(int length, float keepcount, string targetText)
    {
        char[] result = new char[length];
        for (int i = 0; i < keepcount; i++)
        {
            result[i] = targetText[i];
        }
        for (int i = (int)(keepcount % 1 == 0 ? keepcount : keepcount + 1); i < length; i++)
        {
            if (targetText[i] == '\n')
                result[i] = '\n';
            else
                result[i] = randomChars[Random.Range(0, randomChars.Length)];
        }
        return new string(result);
    }
    private void SetTextWindowAlpha(Material material, float t)
    {
        material.SetColor("_FaceColor", Color.Lerp(Color.clear, Color.white, t));
    }
    private void SetBcakWindowAlpha(Material material, float t)
    {
        material.SetFloat(Shader.PropertyToID("_Algha"), Mathf.Lerp(0, 1f, t)); ;
    }
    // private void SetWindowPositionAndScale(Transform transform, Vector3 position, Vector3 scale)
    // {
    //     transform.localScale = scale;
    //     transform.SetLocalPositionAndRotation(position, new Quaternion(0, 0, 0, 0));
    // }
}