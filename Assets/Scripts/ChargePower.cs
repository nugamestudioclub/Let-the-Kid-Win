using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargePower : MonoBehaviour {
	[Range(0.0f, 1.0f)]
	private float charge;

	[SerializeField]
	GameObject fillObject;
	Image image;

	[SerializeField]
	private float _speed = 1.0f;
	private FillState fillState = FillState.Ascending;

	private bool charging;

	void Start() {
		image = fillObject.GetComponent<Image>();
		ResetCharge();
	}

    private void Update()
    {
        if (!charging)
        {
			if (image.fillAmount > 0.0f)
			{
				image.fillAmount -= _speed * Time.deltaTime;
			}
			else image.fillAmount = 0;
        }
    }

    public void ResetCharge() {
		// SetCharge(0.0f);
		charge = 0;
		fillState = FillState.Ascending;
		charging = false;
		GameState.Instance.AudioPlayer.StopPowerCharge();
	}

	public float GetCharge() {
		return charge;
	}

	public float GetScaledCharge(float floor) {
		return floor + (1.0f - floor) * charge;
	}

	public void Tick() {
		if( !charging ) {
			GameState.Instance.AudioPlayer.StartPowerCharge();
			charging = true;
		}
		switch( fillState ) {
		case FillState.Ascending:
			SetCharge(charge + _speed * Time.deltaTime);
			if( charge >= 1.0f ) {
				fillState = FillState.Descending;
			}
			break;

		case FillState.Descending:
			SetCharge(charge - _speed * Time.deltaTime);
			if( charge <= 0.0f ) {
				fillState = FillState.Ascending;
			}
			break;
		}
	}

	private void SetCharge(float value) {
		charge = Mathf.Clamp(value, 0.0f, 1.0f);
		image.fillAmount = charge;
	}

	private enum FillState {
		Ascending, Descending
	}
}