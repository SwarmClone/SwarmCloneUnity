using System.Collections.Generic;
using UnityEngine;
using System.Collections.Concurrent;
using System.Text;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
public class ttsdata
{
    public List<string> Text = new List<string>();
    public List<float> Duration = new List<float>();
    public string emotion;
    public AudioClip Audio;
    public bool isend = false;
}


public class tts_queue : MonoBehaviour
{
    private Dictionary<string,ttsdata> ttsdatadict = new Dictionary<string,ttsdata>();
    private ConcurrentQueue<string>FinshIDs = new ConcurrentQueue<string>();  
    private string ID;
    private string EndID;
    private List<string> TmpIDList = new List<string>();
    public void GetTtsData(Dictionary<string, object> data)
    {
        if ((string)data["source"] == "LLM" && (string)data["message_type"] == "Signal")
            EndID = TmpIDList[TmpIDList.Count - 1];

        ID = data.ContainsKey("id") ? data["id"].ToString() : null;

        if (ID != null && !ttsdatadict.ContainsKey(ID))
        {
            ttsdatadict.Add(data["id"].ToString(), new ttsdata());
            TmpIDList.Add(ID);
        }
        if ((string)data["source"] == "LLM" && (string)data["message_type"] != "Signal")
        {
            ttsdatadict[ID].emotion = getmax((Dictionary<string, object>)data["emotion"]);
        }
        else if ((string)data["source"] == "TTS")
        {
            if (data["data"] is string)
                ttsdatadict[ID].Audio = WavUtility.ToAudioClip(data["data"].ToString());
            else
                ttsdatadict[ID].Audio = WavUtility.ToAudioClip((byte[])data["data"]);
            List<object> aligndatas = (List<object>)data["align_data"];
            foreach (var data1 in aligndatas)
            {
                Dictionary<string, object> align_data = new Dictionary<string, object>();
                align_data = (Dictionary<string, object>)data1;
                foreach (KeyValuePair<string, object> pair in align_data)
                {
                    if(pair.Key == "token")       
                        ttsdatadict[ID].Text.Add(pair.Value.ToString());
                    if(pair.Key == "duration") 
                        ttsdatadict[ID].Duration.Add((float)pair.Value);
                }
            }
            if (ID == EndID)
            {
                ttsdatadict[ID].isend = true;
                foreach (var item in TmpIDList)
                    FinshIDs.Enqueue(item);
                TmpIDList.Clear();
            }
        }
        Debug.Log(TmpIDList.Count);
    }
    private string getmax(Dictionary<string, object> dict)
    {
        string max_key="";float max_value=0;
        foreach(var item in dict)
        {
            if ((float)item.Value > max_value)
            {
                max_key = item.Key;
                max_value = (float)item.Value;
            }
        }
        return max_key;
    }
    public bool PutTtsData(out ttsdata data)
    {
        if(FinshIDs.TryDequeue(out string ID))
        {
            data = ttsdatadict[ID];
            ttsdatadict.Remove(ID);
            return true;    
        }
        else
        {    
            data = null;
            return false;
        }
    }

}