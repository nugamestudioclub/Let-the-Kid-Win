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

	[SerializeField]
	private AudioClip snake;

	[SerializeField]
	private AudioClip[] ladderSteps;

	private int continuousIndex = 1;

	private float currentTime;

	private bool spinning;

	[SerializeField]
	private float timeBetweenSpins;

	private bool climbingLadder;

	[SerializeField]
	private float timeBetweenLadderSteps;

	void Update() {
		if( spinning ) {
			currentTime += Time.deltaTime;
			if( currentTime >= timeBetweenSpins ) {
				PlaySpin();
				currentTime = 0f;
			}
		}
		else if( climbingLadder ) {
			currentTime += Time.deltaTime;
			if( currentTime >= timeBetweenLadderSteps ) {
				PlayLadderStep();
				currentTime = 0f;
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
		currentTime = 0f;
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

	public void PlaySnake() {
		PlayFireAndForget(snake);
	}

	public void PlayLadder() {
		climbingLadder = true;
		currentTime = 0f;
	}

	public void PlayLadderStep() {
		if( !CheckSoundset(ladderSteps, nameof(ladderSteps)) )
			return;
		var audioClip = PickFromSoundset(ladderSteps);
		PlayContinuous(audioClip);
	}

	public void StopLadder() {
		climbingLadder = false;
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