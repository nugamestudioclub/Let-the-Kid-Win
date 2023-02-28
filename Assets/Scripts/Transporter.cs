using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class Transporter : MonoBehaviour {
	[SerializeField]
	private InterpolationSettings interpolationSettings = InterpolationSettings.Default;

	[field: SerializeField]
	public int StartIndex { get; private set; }

	[field: SerializeField]
	public int EndIndex { get; private set; }

	[SerializeField]
	private List<Transform> pointTransforms = new();
	public IReadOnlyList<Transform> PointTransforms => pointTransforms;

	[SerializeField]
	private float transportTime;
	public float TransportTime { get => transportTime; }

	private readonly Path3 path = new();
	public IReadOnlyPath<Vector3> Path => path;

	private void Awake() {
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