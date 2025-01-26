using Live2D.Cubism.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Model_Manager : MonoBehaviour
{
    // 引用Live2D模型组件
    private CubismModel model;

    //观看方向
    [Header("观看方向")]
    public Vector2 look_pos;
    private Vector2 curr_pos, target_pos;
    bool isLooking;



    //说话
    [Header("说话")]
    public ShakeAction_Resources action_talk;
    public bool is_talking;

    public ShakeAction_Resources[] shake_actions;
    public ChangeAction_Resources[] change_actions;
    // 字典用于存储协程的唯一标识符和协程引用
    public Dictionary<string, AnimationValues> action_dictionary_shake = new Dictionary<string, AnimationValues>();
    public Dictionary<string, AnimationValues_Change> action_dictionary_change = new Dictionary<string, AnimationValues_Change>();
    [Header("用于动态修改各个动作属性")]
    public List<AnimationValues> values_List_shake;
    public List<AnimationValues_Change> values_List_change;

    public List<string> test;

    public bool Is_talking
    {
        get => is_talking;
        set
        {
            is_talking = value;
            if (is_talking)
            {
                action_dictionary_shake[action_talk.action_name].isPlaying = true;
            }
            else
            {
                action_dictionary_shake[action_talk.action_name].isPlaying = false;
                StartCoroutine(ResetCoroutine(action_talk.action_name));
            }
        }
    }
    private void Start()
    {

        Find();
        Init();
        foreach (var item in model.Parameters)
        {
            test.Add(item.name);
        }
    }
    private void Find()
    {
        model = GameObject.FindGameObjectWithTag("Live2D_Model").GetComponent<CubismModel>();
        shake_actions = Resources.LoadAll<ShakeAction_Resources>("ShakeActions");
        change_actions = Resources.LoadAll<ChangeAction_Resources>("ChangeActions");
    }
    private void Init()
    {

        InitActions(shake_actions, change_actions);
    }
    private void InitActions(ShakeAction_Resources[] shake_actions, ChangeAction_Resources[] change_actions)
    {
        foreach (var item in shake_actions)
        {
            if (item.actionIndies.Count < 1)
            {
                continue;
            }
            if (item.action_name == "说话")
            {
                action_talk = item;
            }
            StartAction_Shake(item.actionIndies, item.action_name, item.action_speed, item.waitTime, item.waitTime_min, item.waitTime_max, item.action_direction, item.isPlaying);
        }
        foreach (var item in change_actions)
        {
            if (item.actionIndies.Count < 1)
            {
                continue;
            }
            StartAction_Change(item.actionIndies, item.action_name, item.action_speed, item.waitTime, item.waitTime_min, item.waitTime_max, item.min_vector3, item.max_vector3, item.isPlaying);
        }
    }


    private void StartAction_Shake(List<int> valueIndex, string id_name, float changeSpeed, float waitTime, float waitTime_min, float waitTime_max, int dirction, bool isPlaying)
    {
        AnimationValues Values;
        if (!action_dictionary_shake.TryGetValue(id_name, out Values))
        {
            //开启协程传入ID，ID绑定相应的 Values;
            Coroutine coroutine = StartCoroutine(Shake(id_name, isPlaying, dirction));
            Values = new AnimationValues();
            Values.InitValues(coroutine, valueIndex, id_name, changeSpeed, waitTime, waitTime_min, waitTime_max, isPlaying);
            action_dictionary_shake[id_name] = Values;
            values_List_shake.Add(Values);
        }
    }
    private void StartAction_Change(List<int> valueIndex, string id_name, float changeSpeed, float waitTime, float waitTime_min, float waitTime_max, Vector3 min_vector3, Vector3 max_vector3, bool isPlaying)
    {
        AnimationValues_Change Values;
        if (!action_dictionary_change.TryGetValue(id_name, out Values))
        {
            //开启协程传入ID，ID绑定相应的 Values;
            Coroutine coroutine = StartCoroutine(Change(id_name, isPlaying));
            Values = new AnimationValues_Change(coroutine, valueIndex, id_name, changeSpeed, waitTime, waitTime_min, waitTime_max, isPlaying, min_vector3, max_vector3);
            action_dictionary_change[id_name] = Values;
            values_List_change.Add(Values);
        }
    }
    //重置动作
    IEnumerator ResetCoroutine(string id_name)
    {
        AnimationValues values = action_dictionary_shake[id_name];
        List<int> curr_indices = values.action_indices;
        float curr_value = model.Parameters[curr_indices[0]].Value;
        float min_value = model.Parameters[curr_indices[0]].MinimumValue;
        while (curr_value > min_value)
        {
            curr_value -= 0.025f * values.changeSpeed;
            UpdateAnimationValues(values.action_indices, curr_value);
            yield return new WaitForSeconds(0.05f);
        }
        UpdateAnimationValues(curr_indices, min_value);
        yield break;
    }

    //数值向指定目标变化
    IEnumerator Change(string id_name, bool isPlaying)
    {
        AnimationValues_Change values;
        while (!(action_dictionary_change.TryGetValue(id_name, out values) && action_dictionary_change[id_name] != null))
        {
            yield return new WaitForSeconds(0.005f);
        }
        float curr_Value = 0;


        List<int> valueIndex = values.action_indices;
        Vector3 curr_v2;
        Vector3 start_v2 = Vector3.zero, target_v2;
        float waitTime, changeSpeed;
        UpdateValues(id_name, out waitTime, out changeSpeed, out target_v2, out isPlaying);

        while (true)
        {
            if (!isPlaying)
            {
                yield return new WaitForSeconds(0.05f);
                continue;
            }



            curr_Value += 0.01f * changeSpeed;
            curr_v2 = Vector3.Lerp(start_v2, target_v2, curr_Value);
            if (curr_Value >= 1)
            {
                curr_Value = 0;
                start_v2 = target_v2;
                yield return new WaitForSeconds(waitTime);
                UpdateValues(id_name, out waitTime, out changeSpeed, out target_v2, out isPlaying);

            }

            UpdateValues(id_name, out waitTime, out changeSpeed, out isPlaying);
            UpdateAnimationValues(valueIndex, curr_v2);
            yield return new WaitForSeconds(0.05f);
        }
    }
    //数值起伏变化
    IEnumerator Shake(string id_name, bool isPlaying, int direction)
    {
        AnimationValues values;
        while (!(action_dictionary_shake.TryGetValue(id_name, out values) && action_dictionary_shake[id_name] != null))
        {
            yield return new WaitForSeconds(0.005f);
        }
        List<int> valueIndex = values.action_indices;

        int curr_direction = direction;
        float curr_Value = model.Parameters[valueIndex[0]].Value;
        float max_Value = model.Parameters[valueIndex[0]].MaximumValue;
        float min_Value = model.Parameters[valueIndex[0]].MinimumValue;

        float waitTime, changeSpeed;

        int count = 0;
        while (true)
        {
            UpdateValues(id_name, out waitTime, out changeSpeed, out isPlaying);
            if (!isPlaying)
            {
                yield return new WaitForSeconds(0.05f);
                continue;
            }

            if (count >= 2)
            {
                yield return new WaitForSeconds(waitTime);
                count = 0;
            }

            curr_Value = Mathf.Clamp(curr_Value + 0.01f * changeSpeed * curr_direction, min_Value, max_Value)+Random.Range(-0.1f,0.1f);
            if (curr_Value >= max_Value || curr_Value <= min_Value)
            {
                curr_direction *= -1;
                if (waitTime > 0) count++;

            }

            UpdateAnimationValues(valueIndex, curr_Value);
            yield return new WaitForSeconds(0.05f);
        }
    }
    private void UpdateAnimationValues(List<int> valueIndex, float value)
    {
        //设置数值
        if (valueIndex.Count == 1)
        {
            model.Parameters[valueIndex[0]].Value = value;
        }
        else
        {
            foreach (var item in valueIndex)
            {
                model.Parameters[item].Value = value;
            }
        }
    }
    private void UpdateAnimationValues(List<int> valueIndex, Vector3 value)
    {
        model.Parameters[0].Value = value.x;
        model.Parameters[14].Value = value.x/3;
        model.Parameters[1].Value = value.y;
        model.Parameters[15].Value = value.y/3;
        model.Parameters[2].Value = value.z;
        model.Parameters[16].Value = value.z/3  ;

    }
    private void UpdateValues(string id_name, out float curr_waitTime, out float changeSpeed, out bool isPlaying)
    {
        AnimationValues shake_values;
        AnimationValues_Change change_values;
        float  waitTime_min, waitTime_max;

        
        if (action_dictionary_shake.TryGetValue(id_name, out shake_values))
        {
            waitTime_min = shake_values.waitTime_min;
            waitTime_max = shake_values.waitTime_max;
            if (waitTime_min != waitTime_max && waitTime_min >= 0 && waitTime_max > 0)
            {
                curr_waitTime = Random.Range(waitTime_min, waitTime_max);
            }
            else
            {
                curr_waitTime = shake_values.waitTime;
            }
            changeSpeed = shake_values.changeSpeed;
            isPlaying = shake_values.isPlaying;

        }
        else
        {
            action_dictionary_change.TryGetValue(id_name, out change_values);
            waitTime_min = change_values.waitTime_min;
            waitTime_max = change_values.waitTime_max;
            if (waitTime_min != waitTime_max && waitTime_min >= 0 && waitTime_max > 0)
            {
                curr_waitTime = Random.Range(waitTime_min, waitTime_max);
            }
            else
            {
                curr_waitTime = change_values.waitTime;
            }
            curr_waitTime = change_values.waitTime;
            changeSpeed = change_values.changeSpeed;
            isPlaying = change_values.isPlaying;
        }
    }
    private void UpdateValues(string id_name, out float curr_waitTime, out float changeSpeed, out Vector3 target, out bool isPlaying)
    {
        AnimationValues_Change values = action_dictionary_change[id_name];
        if (values.waitTime_min != values.waitTime_max && values.waitTime_min >= 0 && values.waitTime_max > 0)
            curr_waitTime = Random.Range(values.waitTime_min, values.waitTime_max);
        else
            curr_waitTime = values.waitTime;
        target = new Vector3(Random.Range(values.min_vertor3.x, values.max_vertor3.x), Random.Range(values.min_vertor3.y, values.max_vertor3.y), Random.Range(values.min_vertor3.z, values.max_vertor3.z));
        changeSpeed = values.changeSpeed;
        isPlaying = values.isPlaying;
    }
    public float Reset_WaitTime(float minValue, float maxValue)
    {
        return Random.Range(minValue, maxValue);
    }
}
