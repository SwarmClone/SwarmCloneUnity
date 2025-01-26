using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationValues_Change : AnimationValues

{

    public Vector3 min_vertor3;
    public Vector3 max_vertor3;

    public AnimationValues_Change(Coroutine coroutine, List<int> action_indices, string name, float changeSpeed, float waitTime, float waitTime_min, float waitTime_max, bool isPlaying, Vector3 min_vertor3, Vector3 max_vertor3)
    {
        base.InitValues(coroutine, action_indices, name, changeSpeed, waitTime, waitTime_min, waitTime_max, isPlaying);

        this.min_vertor3 = min_vertor3;
        this.max_vertor3 = max_vertor3;
    }
}
