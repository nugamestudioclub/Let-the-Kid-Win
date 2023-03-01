using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Snake : MonoBehaviour {
	[SerializeField]
	private InterpolationSettings interpolationSettings = InterpolationSettings.Default;

	[field: SerializeField]
	public TransportationSettings TransportationSettings { get; private set; }

	[SerializeField]
	private List<Transform> pointTransforms = new();
	public IReadOnlyList<Transform> PointTransforms => pointTransforms;

	private readonly Path3 path = new();
	public IReadOnlyPath<Vector3> Path => path;

	void Awake() {
		path.AddRange(GetPoints());
	}
	
	public void ConnectPoints(Vector3 start, Vector3 end) {
		var points = GetPoints().Prepend(start).Append(end).ToList();
		path.Clear();
		path.AddRange(points);
		Paths.Interpolate(path, interpolationSettings);
	}

	private IEnumerable<Vector3> GetPoints() {
		return pointTransforms.Select(t => t.position).ToList();
	}
}