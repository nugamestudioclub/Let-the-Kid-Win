using System.Collections;
using UnityEngine;

public class AudioPlayer : MonoBehaviour {
	[SerializeField]
	AudioSource[] audioSources;

	[SerializeField]
	private AudioClip powerCharge;

	[SerializeField]
	private AudioClip spinnerStart;
	[SerializeField]
	private AudioClip[] spinnerSpins;

	[SerializeField]
	private AudioClip[] pieceMoves;

	private int continuousIndex = 1;

	private bool spinning;

	private float currentSpinTime;

	[SerializeField]
	private float timeBetweenSpins;

	void Update() {
		if( spinning ) {
			currentSpinTime += Time.deltaTime;
			if( currentSpinTime >= timeBetweenSpins ) {
				PlaySpin();
				currentSpinTime = 0f;
			}
		}
	}

	public void StartPowerCharge() {
		PlayLooping(powerCharge);
	}

	public void StopPowerCharge() {
		StopLooping();
	}

	public void StartSpinner() {
		PlayFireAndForget(spinnerStart);
		spinning = true;
		StartCoroutine(PlayFirstSpin());
		PlaySpin();
	}

	private IEnumerator PlayFirstSpin() {
		yield return new WaitForSeconds(timeBetweenSpins / 2);
		PlaySpin();
	}

	private void PlaySpin() {
		if( !CheckSoundset(spinnerSpins, nameof(spinnerSpins)) )
			return;
		var audioClip = PickFromSoundset(spinnerSpins);
		PlayContinuous(audioClip);
	}

	public void StopSpinner() {
		spinning = false;
	}

	public void MovePiece() {
		if( !CheckSoundset(pieceMoves, nameof(pieceMoves)) )
			return;
		var audioClip = PickFromSoundset(pieceMoves);
		PlayFireAndForget(audioClip);
	}

	private void PlayFireAndForget(AudioClip audioClip) {
		if( !CheckAudioClip(audioClip) | !CheckAudioSources(1) )
			return;
		var audioSource = audioSources[0];
		audioSource.loop = false;
		audioSource.PlayOneShot(audioClip);
	}

	private void PlayContinuous(AudioClip audioClip) {
		if( !CheckAudioClip(audioClip) | !CheckAudioSources(3) )
			return;
		audioSources[continuousIndex].PlayOneShot(audioClip);
		continuousIndex = (continuousIndex % (audioSources.Length - 1)) + 1;
	}

	private void PlayLooping(AudioClip audioClip) {
		if( !CheckAudioClip(audioClip) | !CheckAudioSources(3) )
			return;
		var audioSource = audioSources[0];
		audioSource.clip = audioClip;
		audioSource.loop = true;
		audioSource.Play();
	}

	private void StopLooping() {
		if( !CheckAudioSources(3) )
			return;
		var audioSource = audioSources[0];
		audioSource.Stop();
		audioSource.clip = null;
		audioSource.loop = false;
	}

	private AudioClip PickFromSoundset(AudioClip[] soundset) {
		return soundset[Random.Range(0, soundset.Length - 1)];
	}

	private bool CheckAudioClip(AudioClip audioClip) {
		if( audioClip != null ) {
			return true;
		}
		else {
			Debug.Log($"{nameof(AudioClip)} is null! No audio to play...");
			return false;
		}
	}

	private bool CheckAudioSources(int count) {
		if( audioSources != null && audioSources.Length >= count ) {
			return true;
		}
		else {
			Debug.Log($"{nameof(AudioSource)}s are not serialized!");
			return false;
		}
	}

	private bool CheckSoundset(AudioClip[] audioClips, string name) {
		if( audioClips != null && audioClips.Length > 0 ) {
			return true;
		}
		else {
			Debug.Log($"{nameof(AudioClip)}s not serialized! (Soundset '{name}')");
			return false;
		}
	}
}