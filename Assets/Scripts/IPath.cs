using System.Collections.Generic;
public interface IReadOnlyPath<V> : IReadOnlyList<V> {
	float Length { get; }
	float GetDistanceBetween(int start, int end);
}

public interface IPath<V> : IReadOnlyPath<V>, IList<V> {
	void AddRange(IEnumerable<V> items);
	void InsertRange(int index, IEnumerable<V> items);
	void RemoveRange(int index, int count);
}