using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardSpace : MonoBehaviour {
	[SerializeField]
	private InterpolationSettings interpolationSettings = InterpolationSettings.Default;

	[SerializeField]
	Vector2 offset;

	private readonly Path3 path = new();
	public IReadOnlyPath<Vector3> Path => path;

	public void Connect(Vector3 end, Direction offsetDirection) {
		path.Clear();
		var start = transform.position;
		var midpoint = GetMidpoint(start, end, offsetDirection);
		path.Add(start);
		path.Add(midpoint);
		path.Add(end);
		Paths.Interpolate(path, interpolationSettings);
	}

	private Vector3 GetMidpoint(Vector3 start, Vector3 end, Direction offsetDirection) {
		var midpoint = Vector3.Lerp(start, end, 0.5f);
		switch( offsetDirection ) {
		case Direction.Right: midpoint.x += offset.x; break;
		case Direction.Left: midpoint.x -= offset.x; break;
		case Direction.Up: midpoint.y += offset.y; break;
		case Direction.Down: midpoint.y -= offset.y; break;
		}
		return midpoint;
	}
}