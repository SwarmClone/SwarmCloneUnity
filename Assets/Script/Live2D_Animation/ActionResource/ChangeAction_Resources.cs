using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ChangeAction_Resources : Action_Resources
{
    [Header("头身角度变化区间")]
    public Vector3 min_vector3;
    public Vector3 max_vector3;

}
