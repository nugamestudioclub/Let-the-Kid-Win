using System.Linq;
using UnityEngine;

public static class Paths {
	public static void Interpolate(IPath<Vector3> path, InterpolationSettings interpolationSettings) {
		if( interpolationSettings.Enabled ) {
			var points = path.ToList();
			path.Clear();
			var interpolatedPoints = VectorMath.InterpolateCurve(
				points,
				interpolationSettings.VerticesPerSegment,
				interpolationSettings.Samples
			);
			path.AddRange(interpolatedPoints);
		}
	}
}