using System;
using System.Net.Http.Headers;
using UnityEngine;

[Serializable]
public struct MovementSettings {
	[SerializeField]
	private float durationInSeconds;

	public float DurationInSeconds => durationInSeconds;

	public MovementSettings(float durationInSeconds) {
		if( durationInSeconds < 0 )
			throw new ArgumentOutOfRangeException(nameof(durationInSeconds));
		this.durationInSeconds = durationInSeconds;
	}

	public static readonly MovementSettings Default = new(durationInSeconds: 0f);
}