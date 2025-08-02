using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum objectclass
{
    auidoback = 1,
    cameraback = 2, 
}
public class MeshObject
{
    public GameObject meshobject;
    public objectclass objectclass;
    public Material material;
    public Texture2D texture;

    public MeshObject(GameObject meshobject, objectclass objectclass, Material material, Texture2D texture)
    {
        this.meshobject = meshobject;
        this.objectclass = objectclass;
        this.material = material;
        this.texture = texture;
    }
}

public class MeshObjectController : MonoBehaviour
{
    private static MeshObjectController _instance;
    public static MeshObjectController instance => _instance;
    public new Camera camera;
    public GameObject audioblock;
    public GameObject camerablock;


    private int Bandscount;

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
        Bandscount = AuidoSample.instance.Bandscount;
    }
    public IEnumerator singback()
    {
        Manager.instance.state.MeshObjectController_IsChang = true;
        List<MeshObject> objects = new List<MeshObject>();
        StartCoroutine(CreatAudioBack(objects));
        StartCoroutine(CreatCameraBcak(objects));
        while (!(Manager.instance.state.modelstatu == modelstatu.changing))
            yield return null;
        StartCoroutine(DestroyObjects(objects));
    }
    private IEnumerator CreatCameraBcak(List<MeshObject> objects)
    {
        GameObject new_block = Instantiate(camerablock);
        new_block.transform.SetParent(GetComponent<Transform>());
        new_block.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
        Material material = new_block.GetComponent<MeshRenderer>().materials[0];
        RenderTexture _cameratexture = new RenderTexture(64,36,0);
        camera.targetTexture = _cameratexture;
        material.SetTexture(Shader.PropertyToID("_CameraTexture"), _cameratexture);
        objects.Add(new MeshObject(new_block, objectclass.cameraback, material, null));

        for (float t = 0.02f; t < 1; t += 0.02f)
        {
            material.SetFloat("_Alpha", Mathf.Lerp(0, 1f, t));
            yield return null;
        }
    }
    private IEnumerator CreatAudioBack(List<MeshObject> objects)
    {
        GameObject new_block = Instantiate(audioblock);
        new_block.transform.SetParent(camera.GetComponent<Transform>());
        new_block.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 200), new Quaternion(0, 0, 0, 0));
        Material material = new_block.GetComponent<MeshRenderer>().materials[0];
        Texture2D _audiotexture = new Texture2D(Bandscount, 1);
        _audiotexture.filterMode = FilterMode.Point;
        material.SetTexture(Shader.PropertyToID("_AudioTex"), _audiotexture);
        objects.Add(new MeshObject(new_block, objectclass.auidoback, material, _audiotexture));

        // for (float t = 0.02f; t < 1; t += 0.02f)
        //     yield return null;

        for (float t = 0f; t < 1; t += 0.02f)
        {
            var sample = new float[Bandscount];
            for (var i = 0; i < Bandscount / 2; i++)
            {
                sample[i] = Mathf.Lerp(0, AuidoSample.instance.standBand[i], t);
                sample[Bandscount - i - 1] = sample[i];
            }
            SetTexture(sample, _audiotexture);
            AuidoSample.instance.t = -Mathf.PI;
            yield return null;
        }

        while (Manager.instance.state.modelstatu != modelstatu.changing)
        {
            SetTexture(AuidoSample.instance.audioBandBuffers, _audiotexture);
            yield return null;
        }

    }
    private IEnumerator DestroyObjects(List<MeshObject> meshObjects)
    {
        for (float t = 1f; t >= 0; t -= 0.02f)
        {
            foreach (var meshobject in meshObjects)
            {
                if (meshobject.objectclass == objectclass.auidoback)
                {
                    var sample = new float[Bandscount];
                    for (var i = 0; i < Bandscount; i++)
                        sample[i] = Mathf.Lerp(0, AuidoSample.instance.standBand[i], t);
                    SetTexture(sample, meshobject.texture);
                }
                meshobject.material.SetFloat("_Alpha", Mathf.Lerp(0, 1f, t));
                yield return null;
            }

        }

        foreach (var meshobject in meshObjects)
        {
            Destroy(meshobject.meshobject);
        }
    }
    private void SetTexture(float[] sample, Texture2D texture2D)
    {
        Color col = Color.black;
        for (int i = 0; i < sample.Length; i++)
        {
            col.r = sample[i];
            texture2D.SetPixel(i, 0, col);
        }
        texture2D.Apply();
    }
}
