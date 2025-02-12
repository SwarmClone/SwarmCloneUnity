using Live2D.Cubism.Core;
using Live2D.Cubism.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public Dictionary<string, AnimationValues_Shake> action_dictionary_shake = new Dictionary<string, AnimationValues_Shake>();
    public Dictionary<string, AnimationValues_Change> action_dictionary_change = new Dictionary<string, AnimationValues_Change>();
    [Header("用于动态修改各个动作属性")]
    public List<AnimationValues_Shake> values_List_shake;
    public List<AnimationValues_Change> values_List_change;

    public List<string> test;
    public Action animAction;
    public bool Is_talking
    {
        
        get => is_talking;
        set
        {
            is_talking = value;
            if (is_talking)
            {
                foreach (var action in action_dictionary_shake)
                {
                    action.Value.isPlaying = false;
                }
                foreach (var action in action_dictionary_change)
                {
                    action.Value.isPlaying = false;
                }
                action_dictionary_shake[action_talk.action_name].isPlaying = true;
            }
            else
            {
                foreach (var action in action_dictionary_shake)
                {
                    action.Value.isPlaying = true;
                }
                foreach (var action in action_dictionary_change)
                {
                    action.Value.isPlaying = true;
                }
                action_dictionary_shake[action_talk.action_name].isPlaying = false;
            }
        }
    }
    private bool emotion_play;
    public bool Emotion_play
    {
        get => emotion_play;
        set
        {
            emotion_play = value;
            if (emotion_play)
            {
                foreach (var action in action_dictionary_shake)
                {
                    if(action.Key == action_talk.action_name)
                        continue;
                    action.Value.isPlaying = false;
                }
                foreach (var action in action_dictionary_change)
                {
                    action.Value.isPlaying = false;
                }
            }
            else
            {
                foreach (var action in action_dictionary_shake)
                {
                    if(action.Key == action_talk.action_name)
                        continue;
                    action.Value.isPlaying = true;
                }
                foreach (var action in action_dictionary_change)
                {
                    action.Value.isPlaying = true;
                }
            }
        }
    }
    private void Awake()
    {
        Find();
    }
    private void Start()
    {
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
            AnimationValues_Shake Values = new AnimationValues_Shake(item.actionIndies, item.action_name, item.action_speed, item.waitTime, item.waitTime2, item.waitTime_min, item.waitTime_max, item.isPlaying, model, item.action_direction);
            animAction += Values.StartAction;
            action_dictionary_shake[item.action_name] = Values;
            values_List_shake.Add(Values);
        }
        foreach (var item in change_actions)
        {
            if (item.actionIndies.Count < 1)
            {
                continue;
            }
            AnimationValues_Change Values = new AnimationValues_Change(item.actionIndies, item.action_name, item.action_speed, item.waitTime, item.waitTime_min, item.waitTime_max, item.isPlaying, model);
            animAction += Values.StartAction;
            action_dictionary_change[item.action_name] = Values;
            values_List_change.Add(Values);
        }
    }
    private void Update()
    {
        Emotion_play = motioncont_test.instance.motionController.IsPlayingAnimation();
        animAction?.Invoke();
        model.ForceUpdateNow();
    }
}
