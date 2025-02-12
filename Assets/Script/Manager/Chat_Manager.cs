using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using TMPro;
using UnityEngine;

public class Chat_Manager : MonoBehaviour
{
    public TMP_Text chat_text;
    public float text_wait_time=0.1f;
    public class idinform
    {
        public List<string> Text = new List<string>();
        public List<float> Voice = new List<float>();
        public string emotion;
    }
    public Dictionary<string, idinform> IDdict = new Dictionary<string,idinform>();
    public ConcurrentQueue<string>IDlist = new ConcurrentQueue<string>();
    public Model_Manager model_Manager;
    public request_queue request_Queue;
    public float enter_timer;
    private void Start()
    {
        StartCoroutine(GetMSG());
    }


    private void Update()
    {
        if (enter_timer>0)
        {
            enter_timer -= Time.deltaTime;
            if (enter_timer<=0)
            {
                chat_text.text = "";
            }
        }
    }

    public void SetSomeText()
    {
        List<string> ids = new List<string>();
        chat_text.text = "";
        while(!IDlist.IsEmpty)
        {
            string id;
            IDlist.TryDequeue(out id);
            ids.Add(id);
        }
        print("文本输入");
        StartCoroutine(Model_talking(ids));
  
     
    }
    IEnumerator Model_talking(List<string> ids)
    {
        foreach(var id in ids)
        {
            for(int i=0;i<IDdict[id].Text.Count;i++)
            {
                chat_text.text += IDdict[id].Text[i];
                model_Manager.Is_talking = true;
                motioncont_test.instance.Motion=IDdict[id].emotion;

                enter_timer = 5;
                yield return new WaitForSeconds(IDdict[id].Voice[i]);
            }
            IDdict.Remove(id);
        }
        model_Manager.Is_talking = false;
        yield break;
    }
    IEnumerator GetMSG()
    {   
        
        while(true)
        {   
            if(!request_Queue.isempty())
            {   
                RecvData msg = request_Queue.ShareData;
                if(msg.from == "llm" && msg.type == "data")
                {
                    IDlist.Enqueue(msg.payload["id"].ToString());
                    IDdict.Add(msg.payload["id"].ToString(),new idinform());
                    IDdict[msg.payload["id"].ToString()].emotion = getemotion((Dictionary<string, float>)msg.payload["emotion"]);
                }
                else if(msg.from == "tts" && msg.type == "data")
                {
                    IDdict[msg.payload["id"].ToString()].Text.Add(msg.payload["token"].ToString());
                    IDdict[msg.payload["id"].ToString()].Voice.Add((float)msg.payload["duration"]);
                }
                else if(msg.from == "tts" && msg.type == "signal" && msg.payload2 == "finish")
                {   
                    SetSomeText();
                }
            }
            yield return new WaitForSeconds(0.001f);
        }
    }

    private string getemotion(Dictionary<string, float> dict)
    {
        string max_key="";float max_value=0;
        foreach(var item in dict)
        {
            if(item.Value>max_value)
            {
                max_key = item.Key;
                max_value = item.Value;
            }
        }
        return max_key;
    }
}
