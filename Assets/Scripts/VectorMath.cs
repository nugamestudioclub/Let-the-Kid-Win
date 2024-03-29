﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class VectorMath {
	// Lagrange interpolation with x as independent variable
	private static Vector3 InterpolateX(IList<Vector3> points, float x) {
		float y = 0f;
		for( int i = 0; i < points.Count; ++i ) {
			float term = points[i].y;
			for( int j = 0; j < points.Count; ++j )
				if( Math.Abs(points[i].x - points[j].x) > float.Epsilon
					&& Math.Abs(x - points[j].x) > float.Epsilon )
					term *= (x - points[j].x) / (points[i].x - points[j].x);
			y += term;
		}
		return new(x, y);
	}

	// Lagrange interpolation with y as independent variable
	private static Vector3 InterpolateY(IList<Vector3> points, float y) {
		float x = 0f;
		for( int i = 0; i < points.Count; ++i ) {
			float term = points[i].x;
			for( int j = 0; j < points.Count; ++j )
				if( Math.Abs(points[i].y - points[j].y) > float.Epsilon
					&& Math.Abs(y - points[j].y) > float.Epsilon )
					term *= (y - points[j].y) / (points[i].y - points[j].y);
			x += term;
		}
		return new(x, y);
	}

	public static IEnumerable<Vector3> InterpolateSegment(IList<Vector3> points, int samples) {
		var start = points[0];
		var end = points[^1];
		var delta = new Vector3(end.x - start.x, end.y - start.y);
		int vertexCount = points.Count * samples;

		if( Math.Abs(delta.x) > Math.Abs(delta.y) ) {
			float epsilon = delta.x / vertexCount;
			yield return start;
			for( int i = 1; i < vertexCount; ++i )
				yield return InterpolateX(points, start.x + (i * epsilon));
		}
		else {
			float epsilon = delta.y / vertexCount;
			yield return start;
			for( int i = 1; i < vertexCount; ++i )
				yield return InterpolateY(points, start.y + (i * epsilon));
		}
	}


	/*
	(n - 1) / (k - 1)

	1, 1:  0

	2, 1:  0 1

	3, 1:  0 1 2

	4, 1:  0 1 2 3

	5, 2:  0 1 2
	           2 3 4

	6, 2:  0 1 2
	           2 3 4 5

	7, 3:  0 1 2
	           2 3 4
	               4 5 6
	8, 3:  0 1 2
	           2 3 4
	               4 5 6 7

	9, 4:  0 1 2
	           2 3 4
	               4 5 6
	                   6 7 8
	10, 4: 0 1 2
	           2 3 4
	               4 5 6
	                   6 7 8 9
	11, 5: 0 1 2
	           2 3 4
	               4 5 6
	                   6 7 8
	                       8 9 10
	 */

	public static IEnumerable<Vector3> InterpolateCurve(IEnumerable<Vector3> points, int verticesPerSegment, int samples) {
		var sourcePoints = new List<Vector3>(points);
		// Prune overlapping vertices
		for( int i = sourcePoints.Count - 1; i > 0; --i )
			if( Math.Abs(Vector3.Distance(sourcePoints[i], sourcePoints[i - 1])) <= float.Epsilon )
				sourcePoints.RemoveAt(i);
		int segments = (sourcePoints.Count - 1) / (verticesPerSegment - 1);
		int startIndex = 0;
		// First n-1 segments
		for( int i = 0; i < segments - 1; ++i ) {
			foreach( var point in InterpolateSegment(sourcePoints.GetRange(startIndex, verticesPerSegment), samples) )
				yield return point;
			startIndex += verticesPerSegment - 1;
		}
		// Last segment (including extra vertices)
		foreach( var point in InterpolateSegment(sourcePoints.GetRange(startIndex, sourcePoints.Count - startIndex), samples) )
			yield return point;
		// Endpoint
		yield return sourcePoints[^1];
	}
}