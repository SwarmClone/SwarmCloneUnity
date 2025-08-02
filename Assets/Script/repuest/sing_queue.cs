using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;
using System.Collections;

public class singdata
{
    public string songname;
    public AudioClip songclip;
    public List<KeyValuePair<string, List<float>>> subtitles;
}

public class sing_queue : MonoBehaviour
{
    private ConcurrentQueue<singdata> singdatas = new ConcurrentQueue<singdata>();
    private string songpath = Path.Combine(Application.dataPath, "Audio");

    void Start()
    {
        Debug.Log(songpath);
    }

    public void GetSingData(Dictionary<string, object> data)
    {
        singdata song = new singdata();
        song.songname = (string)data["song_id"];
        song.subtitles = GetSubTitle((string)data["subtitle_path"]);
        StartCoroutine(GetAudioClip((string)data["song_path"],(audioClip) =>
        {
            song.songclip = audioClip;
            singdatas.Enqueue(song);
            Debug.Log("plugin get");
        }));
        
    }
    public bool PutSingData(out singdata data)
    {
        if (singdatas.IsEmpty)
        {
            data = null;
            return false;
        }
        else
        {
            singdatas.TryDequeue(out data);
            Debug.Log("plugin put");
            return true;
        }
    }

    private IEnumerator GetAudioClip(string filename, Action<AudioClip> callback)
    {
        
        string url = "file://"+Path.Combine(songpath, filename);
        Debug.Log(url);
        UnityWebRequest request = new UnityWebRequest();
        switch (filename.Split(".")[1])
        {
            case "wav": request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV); break;
            case "mp3": request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG); break;
        }
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.Success)
        {
            AudioClip audioClip = DownloadHandlerAudioClip.GetContent(request);
            callback?.Invoke(audioClip);
        }
        else
        {
            Debug.LogError("加载音频失败: " + request.error);
            callback?.Invoke(null);
        }
    }
    private List<KeyValuePair<string, List<float>>> GetSubTitle(string filename)
    {
        string filePath = Path.Combine(songpath, filename);
        List<KeyValuePair<string, List<float>>> result = new List<KeyValuePair<string, List<float>>>();
        string[] lines = File.ReadAllLines(filePath);

        for (int i = 0; i < lines.Length; i++)
        {
            // 跳过空行和序号行
            if (string.IsNullOrWhiteSpace(lines[i]) || int.TryParse(lines[i], out int _))
                continue;

            // 提取时间戳行
            if (lines[i].Contains("-->"))
            {
                string[] timeParts = lines[i].Split("-->", StringSplitOptions.None);
                if (timeParts.Length != 2)
                    continue;

                // 解析开始时间戳
                float startTime = ParseTime(timeParts[0].Trim());
                float endTime = ParseTime(timeParts[1].Trim());
                // 提取字幕文本
                string text = "";
                i++;
                while (i < lines.Length && !string.IsNullOrWhiteSpace(lines[i]) && !lines[i].Contains("-->"))
                {
                    text += lines[i] + "\n";
                    i++;
                }
                text = text.Trim(); // 去掉多余的换行符

                // 将解析结果存储到字典中
                result.Add(new KeyValuePair<string, List<float>>(text, new List<float> { startTime, endTime }));
            }
        }
        return result;
    }
    private float ParseTime(string time)
    {
        string[] parts = time.Split(':');
        int hours = int.Parse(parts[0]);
        int minutes = int.Parse(parts[1]);
        string[] secondsParts = parts[2].Split(',');
        int seconds = int.Parse(secondsParts[0]);
        int milliseconds = int.Parse(secondsParts[1]);

        return hours * 3600 + minutes * 60 + seconds + milliseconds / 1000.0f;
    }
}
