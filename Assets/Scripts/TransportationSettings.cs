using System;
using UnityEngine;

[Serializable]
public struct TransportationSettings {
	[field: SerializeField]
	public int StartIndex { get; private set; }

	[field: SerializeField]
	public int EndIndex { get; private set; }

	[field: SerializeField]
	public float DurationInSeconds { get; private set; }
}