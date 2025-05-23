using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class danmakucontroller : MonoBehaviour
{
    public danmaku_queue danmaku_Queue;

    public GameObject p_danmaku;
    public GameObject ui_danmakulist;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        DanmakuData danmaku;
        if (danmaku_Queue.PutDanmaku(out danmaku))
        {
            DanmakuHandler(danmaku);
        }
    }

    void DanmakuHandler(DanmakuData data)
    {
        GameObject new_danmaku = Instantiate(p_danmaku);
        new_danmaku.transform.SetParent( ui_danmakulist.transform);
        new_danmaku.GetComponent<TMP_Text>().text = data.user + ':' + data.content;
    }
}
