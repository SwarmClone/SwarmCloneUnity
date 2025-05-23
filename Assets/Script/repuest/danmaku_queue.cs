using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

public class DanmakuData
{
    public string user;
    public string content;
}
public class danmaku_queue : MonoBehaviour
{
    private ConcurrentQueue<DanmakuData> DanmakuList = new ConcurrentQueue<DanmakuData>();
    public void GetDanmaku(Dictionary<string, object> data)
    {
        var danmaku = new DanmakuData();
        danmaku.user = (string)data["user"];
        danmaku.content = (string)data["content"];
        DanmakuList.Enqueue(danmaku);
    }
    public bool PutDanmaku(out DanmakuData data)
    {
        if(DanmakuList.TryDequeue(out DanmakuData danmaku))
        {
            data = danmaku;
            return true;    
        }
        else
        {    
            data = null;
            return false;
        }
    }

}
