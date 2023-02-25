using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private List<Transform> points = new();
    public List<Transform> Points => points;

    //public List<float> 

    [SerializeField]
    private float gizmoPointSize = 0.25f;

    [SerializeField]
    private int transportTime;
    public int TransportTime { get => transportTime; }

    private void Awake()
    {
        
    }

    private void OnDrawGizmosSelected()
    {
        foreach (var point in points)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(point.position, gizmoPointSize);
        }
    }

    public float DistanceToNextPoint(int index)
    {

    }
    public float TotalLength()
    {
        float currentTotal = 0;
        List<Vector2> pointPositions = points.Select(p => (Vector2) p.position).ToList();
        for (int i = 0; i < pointPositions.Count - 1; i++)
        {
            currentTotal += Vector2.Distance(pointPositions[i], pointPositions[i+1]);
        }
        return currentTotal;
    }


}
