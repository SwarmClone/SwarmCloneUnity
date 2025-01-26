using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationValues

{
    public Coroutine coroutine;
    [DisplayOnly]
    public List<int> action_indices;
    [Header("名称")]
    [DisplayOnly]
    public string name;

    public float changeSpeed;
    public float waitTime;
    public float waitTime_min;
    public float waitTime_max;
    public bool isPlaying;

    public virtual void InitValues(Coroutine coroutine, List<int> action_indices, string name, float changeSpeed, float waitTime, float waitTime_min, float waitTime_max, bool isPlaying)
    {
        this.coroutine = coroutine;
        this.action_indices = action_indices;
        this.name = name;
        this.changeSpeed = changeSpeed;
        this.waitTime = waitTime;
        this.waitTime_min = waitTime_min;
        this.waitTime_max = waitTime_max;
        this.isPlaying = isPlaying;
    }
}
