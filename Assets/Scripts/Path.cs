using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path3 : IPath<Vector3> {
	private readonly List<Vector3> points;

	private readonly List<float> nextPointDistances;

	public int Count => points.Count;

	public bool IsReadOnly => false;

	private float length;
	public float Length => length;

	public Vector3 this[int index] {
		get => points[index];
		set => points[index] = value;
	}

	public Path3() {
		points = new();
		nextPointDistances = new();
	}

	public Path3(IEnumerable<Vector3> points) {
		this.points = new(points);
		nextPointDistances = new(CalcEachDistanceToNextPoint());
		length = CalcLength();
	}

	public void Add(Vector3 item) {
		points.Add(item);
		if( Count > 1 )
			nextPointDistances[^1] = Vector3.Distance(points[^2], points[^1]);
		nextPointDistances.Add(0f);
		length = CalcLength();
	}

	public void AddRange(IEnumerable<Vector3> items) {
		points.AddRange(items);
		Recalculate();
	}

	private IEnumerable<float> CalcEachDistanceToNextPoint() {
		for( int i = 0; i < points.Count - 1; i++ )
			yield return Vector3.Distance(points[i], points[i + 1]);
		yield return 0f;
	}

	private float CalcLength() {
		float length = 0f;
		for( int i = 0; i < nextPointDistances.Count - 1; ++i )
			length += nextPointDistances[i];
		return length;
	}

	public void Clear() {
		points.Clear();
		nextPointDistances.Clear();
		length = 0f;
	}

	public bool Contains(Vector3 item) {
		return points.Contains(item);
	}

	public void CopyTo(Vector3[] array, int arrayIndex) {
		points.CopyTo(array, arrayIndex);
	}

	public float GetDistanceBetween(int start, int end) {
		return end == start + 1
			? nextPointDistances[start]
			: Vector3.Distance(points[start], points[end]);
	}

	public IEnumerator<Vector3> GetEnumerator() {
		return points.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator() {
		return points.GetEnumerator();
	}

	public int IndexOf(Vector3 item) {
		return points.IndexOf(item);
	}

	public void Insert(int index, Vector3 item) {
		points.Insert(index, item);
		if( index + 1 < points.Count )
			nextPointDistances.Insert(index, Vector3.Distance(points[index], points[index + 1]));
		else
			nextPointDistances.Add(0f);
		if( index > 0 )
			nextPointDistances[index - 1] = Vector3.Distance(points[index - 1], points[index]);
		length = CalcLength();
	}
	public void InsertRange(int index, IEnumerable<Vector3> items) {
		points.InsertRange(index, items);
		Recalculate();
	}

	private void Recalculate() {
		nextPointDistances.Clear();
		nextPointDistances.AddRange(CalcEachDistanceToNextPoint());
		length = CalcLength();
	}

	public bool Remove(Vector3 item) {
		int index = IndexOf(item);
		if( index >= 0 ) {
			RemoveAt(index);
			return true;
		}
		else {
			return false;
		}
	}

	public void RemoveAt(int index) {
		points.RemoveAt(index);
		nextPointDistances.RemoveAt(index);
		if( index + 1 < points.Count )
			nextPointDistances[index] = Vector3.Distance(points[index], points[index + 1]);
		else
			nextPointDistances[index] = 0f;
		if( index > 0 )
			nextPointDistances[index - 1] = Vector3.Distance(points[index - 1], points[index]);
		length = CalcLength();
	}

	public void RemoveRange(int index, int count) {
		points.RemoveRange(index, count);
		Recalculate();
	}
}