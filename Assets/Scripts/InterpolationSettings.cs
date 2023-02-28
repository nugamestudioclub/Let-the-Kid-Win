using System;
using UnityEngine;

[Serializable]
public struct InterpolationSettings {
	[SerializeField]
	private bool enabled;
	public bool Enabled => enabled;

	[SerializeField]
	private int verticesPerSegment;
	public int VerticesPerSegment => verticesPerSegment;

	[SerializeField]
	private int samples;
	public int Samples => samples;

	public InterpolationSettings(bool enabled, int verticesPerSegment, int samples) {
		if( verticesPerSegment < 2 )
			throw new ArgumentOutOfRangeException(nameof(verticesPerSegment));
		if( samples < 0 )
			throw new ArgumentOutOfRangeException(nameof(samples));
		this.enabled = enabled;
		this.verticesPerSegment = verticesPerSegment;
		this.samples = samples;
	}

	public readonly static InterpolationSettings Default = new(true, 3, 3);
}