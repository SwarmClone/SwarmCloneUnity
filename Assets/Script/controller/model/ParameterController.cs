using UnityEngine;
using Live2D.Cubism.Framework;
using Live2D.Cubism.Core;
using Live2D.Cubism.Framework.LookAt;
using System.Data;
using Unity.Mathematics;
using Random = UnityEngine.Random;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;

public enum parameterclass
{
    bodyparameter = 0,
    eyeparameter = 1,
    breathparameter = 2,
    microphoneparameter = 3,
}

public class ParameterController : MonoBehaviour, ICubismUpdatable
{
    private ControledParameter[] Sources { get; set; }
    private CubismParameter[] Destinations { get; set; }
    private Vector3[] LastPositions = new Vector3[3];
    private Vector3[] GoalPositions = new Vector3[3];
    private Vector3[] VelocityBuffers = new Vector3[3];
    private float[] times = new float[3];

    public float breathspeed = 2f;
    public float Damping = 0.15f;
    public CubismParameterBlendMode BlendMode = CubismParameterBlendMode.Additive;
    [HideInInspector]
    public bool HasUpdateController { get; set; }

    void Start()
    {
        Refresh();
    }
    public void Refresh()
    {
        var model = GetComponent<CubismModel>();

        Sources = model
            .Parameters
            .GetComponentsMany<ControledParameter>();
        Destinations = new CubismParameter[Sources.Length];


        for (var i = 0; i < Sources.Length; ++i)
        {
            Destinations[i] = Sources[i].GetComponent<CubismParameter>();
        }

        HasUpdateController = GetComponent<CubismUpdateController>() != null;
    }
    void Update()
    {
        if (times[(int)parameterclass.bodyparameter] <= 0f)
        {
            var sample = GenerateGaussianRandomPosition();
            GoalPositions[0] = new Vector3(
                (float)(sample.x / 2 > 0 ? sample.x / 2 + 0.1 : sample.x / 2 - 0.1),
                (float)(sample.y / 4 > 0 ? sample.y / 4 + 0.1 : sample.y / 4 - 0.1));
            times[(int)parameterclass.bodyparameter] = (CalculateProbabilityDensity(sample.x) * 5f + CalculateProbabilityDensity(sample.y)) * 5f;
        }
        else
            times[(int)parameterclass.bodyparameter] -= Time.deltaTime;

        if (times[(int)parameterclass.eyeparameter] <= 0f)
        {
            var sample = GenerateGaussianRandomPosition();
            GoalPositions[1] = new Vector3(GoalPositions[0].x + sample.x / 2, GoalPositions[0].y + sample.y / 4);
            times[(int)parameterclass.eyeparameter] = (CalculateProbabilityDensity(sample.x) * 2f + CalculateProbabilityDensity(sample.y)) * 2f;
        }
        else
            times[(int)parameterclass.eyeparameter] -= Time.deltaTime;

        times[(int)parameterclass.breathparameter] += Time.deltaTime * breathspeed;
        if (times[2] > 2 * Mathf.PI)
            times[2] -= 2 * Mathf.PI;

        if (Manager.instance.state.modelstatu == modelstatu.singing)
            GoalPositions[2] = Vector3.right;
        else
            GoalPositions[2] = Vector3.zero;

    }
    public void OnLateUpdate()
    {
        if (!enabled || Destinations == null)
        {
            return;
        }

        var positions = LastPositions;
        for (int i = 0; i < positions.Length; i++)
        {
            if (positions[i] != GoalPositions[i])
            {
                positions[i] = Vector3.SmoothDamp(
                    positions[i],
                    GoalPositions[i],
                    ref VelocityBuffers[i],
                    Damping);
            }
        }


        for (var i = 0; i < Destinations.Length; ++i)
        {
            switch (Sources[i].parameterclass)
            {
                case parameterclass.bodyparameter:
                    Destinations[i].BlendToValue(BlendMode, Sources[i].TickAndEvaluate(positions[0])); break;
                case parameterclass.eyeparameter:
                    Destinations[i].BlendToValue(BlendMode, Sources[i].TickAndEvaluate(positions[1])); break;
                case parameterclass.breathparameter:
                    Destinations[i].BlendToValue(BlendMode, (Mathf.Sin(times[2]) + 1) * 0.5f); break;
                case parameterclass.microphoneparameter:
                    Destinations[i].BlendToValue(BlendMode, Sources[i].TickAndEvaluate(positions[2])); break;
            }
        }


        LastPositions = positions;
    }
    public int ExecutionOrder
    {
        get { return CubismUpdateExecutionOrder.CubismLookController + 1; }
    }


    public bool NeedsUpdateOnEditing
    {
        get { return false; }
    }

    private float CalculateProbabilityDensity(float x)
    {
        float coefficient = 1.0f / Mathf.Sqrt(2.0f * Mathf.PI);
        float exponent = Mathf.Exp(-Mathf.Pow(x, 2) / 2);
        return coefficient * exponent;
    }
    private Vector2 GenerateGaussianRandomPosition()
    {
        float u1 = Random.value;
        float u2 = Random.value;

        float z0 = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Cos(2.0f * Mathf.PI * u2);
        float z1 = Mathf.Sqrt(-2.0f * Mathf.Log(u1)) * Mathf.Sin(2.0f * Mathf.PI * u2);

        return new Vector2(z0, z1);
    }
}

