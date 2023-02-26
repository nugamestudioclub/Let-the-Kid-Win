using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Transporter : MonoBehaviour
{
    [SerializeField]
    private int startIndex;
    public int StartIndex { get => startIndex; }
    [SerializeField]
    private int endIndex;
    public int EndIndex { get => endIndex; }

    [SerializeField]
    bool useInterpolated = true;

    [SerializeField]
    private List<Transform> points = new();
    public List<Vector3> Points
    {
        get
        {
            if (runtimePoints.Count == 0)
            {
                runtimePoints = useInterpolated
             ? InterpolatePoints()
             : points.Select(p => p.position).ToList();
            }

            return runtimePoints;
        }

    }


    private List<Vector3> runtimePoints = new();

    private List<float> distanceBetweenPoints = new();

    [SerializeField]
    private float gizmoPointSize = 0.25f;

    [SerializeField]
    private int transportTime;
    public int TransportTime { get => transportTime; }

    private void Awake()
    {
        CalculateDistanceBetweenPoints();
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var point in points)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(point.position, gizmoPointSize);
        }

        Gizmos.color = Color.blue;
        foreach (var point in Points)
            Gizmos.DrawWireSphere(point, gizmoPointSize / 2);
    }

    private List<Vector3> InterpolatePoints()
    {
        Debug.Log($"Calling interpolate points");
        var curvePoints = points.Select(t => t.position).ToList();
        return VectorMath.InterpolateCurve(
            curvePoints,
            segmentLength: 3,
            samplesPerSegment: 24).ToList();
    }

    private void CalculateDistanceBetweenPoints()
    {
        Debug.Log($"Num points in distance calc: {Points.Count}");
        for (int i = 0; i < Points.Count - 1; i++)
        {
            float distanceBetweenNextPoint = Vector3.Distance(Points[i], Points[i + 1]);
            Debug.Log($"Distance between next point: {distanceBetweenNextPoint}");
            distanceBetweenPoints.Add(distanceBetweenNextPoint);
        }
        Debug.Log($"Num distance points : {distanceBetweenPoints.Count}");
        Debug.Log($"Total length : {TotalLength}");
    }
    public float DistanceToNextPoint(int index)
    {
        return distanceBetweenPoints[index];
    }
    public float TotalLength => distanceBetweenPoints.Sum();


}
