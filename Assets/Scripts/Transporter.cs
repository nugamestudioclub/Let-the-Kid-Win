using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Transporter : MonoBehaviour {
	[field: SerializeField]
	public int StartIndex { get; private set; }

	[field: SerializeField]
	public int EndIndex { get; private set; }

	[SerializeField]
	private bool useInterpolation = true;

	[SerializeField]
	private int segmentLength = 3;

	[SerializeField]
	private int samplesPerSegment = 9;

	[SerializeField]
	private List<Transform> pointTransforms = new();
	public IReadOnlyList<Transform> PointTransforms => pointTransforms;

	private List<Vector3> points = new();
	public IReadOnlyList<Vector3> Points => points;

	private List<float> distanceBetweenPoints = new();

	[SerializeField]
	private int transportTime;
	public int TransportTime { get => transportTime; }

	private void Awake() {
		points = GetPoints().ToList();
		CalculateDistanceBetweenPoints();
	}
	
	public void ConnectPoints(Vector3 start, Vector3 end) {
		var points = GetPoints().Prepend(start).Append(end).ToList();
		this.points.Clear();
		this.points.AddRange(useInterpolation
			? VectorMath.InterpolateCurve(points, segmentLength, samplesPerSegment)
			: points
		);
		CalculateDistanceBetweenPoints();
	}

	private IEnumerable<Vector3> GetPoints() {
		return pointTransforms.Select(t => t.position).ToList();
	}

	private void CalculateDistanceBetweenPoints() {
		// Debug.Log($"Num points in distance calc: {Points.Count}");
		distanceBetweenPoints.Clear();
		for( int i = 0; i < Points.Count - 1; i++ ) {
			float distanceBetweenNextPoint = Vector3.Distance(Points[i], Points[i + 1]);
			// Debug.Log($"Distance between next point: {distanceBetweenNextPoint}");
			distanceBetweenPoints.Add(distanceBetweenNextPoint);
		}
		// Debug.Log($"Num distance points : {distanceBetweenPoints.Count}");
		// Debug.Log($"Total length : {TotalLength}");
	}
	public float DistanceToNextPoint(int index) {
		return distanceBetweenPoints[index];
	}
	public float TotalLength => distanceBetweenPoints.Sum();


}
