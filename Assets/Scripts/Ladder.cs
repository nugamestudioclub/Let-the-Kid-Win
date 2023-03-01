using System.Collections.Generic;
using UnityEngine;

public class Ladder : MonoBehaviour {
	[field: SerializeField]
	public TransportationSettings TransportationSettings { get; private set; }

	[SerializeField]
	private int steps;

	private readonly Path3 path = new();
	public IReadOnlyPath<Vector3> Path => path;

	public void ConnectPoints(Vector3 startPoint, Vector3 endPoint) {
		path.Clear();
		path.Add(startPoint);
		float totalDistance = Vector3.Distance(startPoint, endPoint);
		float delta = totalDistance / (steps + 1);
		float currentDistance = delta;
		var currentStep = startPoint;
		for( int i = 0; i <= steps; ++i ) {
			var nextStep = Vector3.Lerp(startPoint, endPoint, currentDistance / totalDistance);
			currentStep = new Vector3(currentStep.x, nextStep.y);
			path.Add(currentStep);
			path.Add(nextStep);
			currentStep = nextStep;
			currentDistance += delta;
		}
	}
}