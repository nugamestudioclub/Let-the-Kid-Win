using System.Collections.Generic;
public interface IReadOnlyPath<T, V> : IReadOnlyList<V> {
	T Length { get; }
	T GetDistanceBetween(int start, int end);
}

public interface IPath<T, V> : IReadOnlyPath<T, V>, IList<V> {
	void AddRange(IEnumerable<V> items);
	void InsertRange(int index, IEnumerable<V> items);
	void RemoveRange(int index, int count);
}