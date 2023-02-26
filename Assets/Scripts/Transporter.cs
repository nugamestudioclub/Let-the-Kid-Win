using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Transporter : MonoBehaviour {
	[SerializeField]
	private int startIndex;
	public int StartIndex { get => startIndex; }
	[SerializeField]
	private int endIndex;
	public int EndIndex { get => endIndex; }

	[SerializeField]
	private List<Transform> points = new();
	public List<Transform> Points => points;

	private List<float> distanceBetweenPoints = new();

	[SerializeField]
	private float gizmoPointSize = 0.25f;

	[SerializeField]
	private int transportTime;
	public int TransportTime { get => transportTime; }

	private void Awake() {
		CalculateDistanceBetweenPoints();
	}

	private void OnDrawGizmosSelected() {
		foreach( var point in points ) {
			Gizmos.color = Color.white;
			Gizmos.DrawWireSphere(point.position, gizmoPointSize);
		}
		var curvePoints = points.Select(t => t.position).ToArray();

		Gizmos.color = Color.blue;
		foreach( var point in VectorMath.InterpolateCurve(curvePoints, 3, 24) )
			Gizmos.DrawWireSphere(point, gizmoPointSize / 2);
	}

	private void CalculateDistanceBetweenPoints() {
		List<Vector2> pointPositions = points.Select(p => (Vector2)p.position).ToList();
		for( int i = 0; i < pointPositions.Count - 1; i++ ) {
			distanceBetweenPoints.Add(Vector2.Distance(pointPositions[i], pointPositions[i + 1]));
		}
	}
	public float DistanceToNextPoint(int index) {
		return distanceBetweenPoints[index];
	}
	public float TotalLength => distanceBetweenPoints.Sum();


}
