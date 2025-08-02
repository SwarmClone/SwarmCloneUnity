using System.Collections;
using System.Collections.Generic;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.LookAt;
using UnityEngine;

public class ControledParameter : MonoBehaviour
{
        [SerializeField]
        public CubismLookAxis Axis;
        public parameterclass parameterclass;
        public float Factor;

    private void Reset()
    {
        var parameter = GetComponent<CubismParameter>();


        // Fail silently.
        if (parameter == null)
        {
            return;
        }


        // Guess axis.
        if (parameter.name.EndsWith("Y"))
        {
            Axis = CubismLookAxis.Y;
        }
        else if (parameter.name.EndsWith("Z"))
        {
            Axis = CubismLookAxis.Z;
        }
        else
        {
            Axis = CubismLookAxis.X;
        }


        // Guess factor.
        Factor = parameter.MaximumValue;
    }
    public float TickAndEvaluate(Vector3 targetOffset)
        {
            var result = (Axis == CubismLookAxis.X)
                ? targetOffset.x
                : targetOffset.y;


            if (Axis == CubismLookAxis.Z)
            {
                result = targetOffset.z;
            }


            return result * Factor;
        }
}
