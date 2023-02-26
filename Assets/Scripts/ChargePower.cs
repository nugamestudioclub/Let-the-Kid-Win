using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChargePower : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    private float charge;

    [SerializeField]
    GameObject fillObject;
    Image image;

    [SerializeField]
    private float _speed = 1.0f;
    private FillState fillState = FillState.Ascending;

    // Start is called before the first frame update
    void Start()
    {
        image = fillObject.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetCharge()
    {
        SetCharge(0.0f);
        fillState = FillState.Ascending;
    }

    public float GetCharge()
    {
        return charge;
    }

    public void Tick()
    {
        switch (fillState) 
        {
            case FillState.Ascending:
                SetCharge(charge + _speed * Time.deltaTime);
                if (charge >= 1.0f)
                {
                   fillState = FillState.Descending;
                }
                break;

            case FillState.Descending:
                SetCharge(charge - _speed * Time.deltaTime);
                if (charge <= 0.0f)
                {
                    fillState = FillState.Ascending;
                }
                break;
        }
    }

    private void SetCharge(float value)
    {
        charge = Mathf.Clamp(value, 0.0f, 1.0f);
        image.fillAmount = charge;
    }

    private enum FillState
    {
        Ascending, Descending
    }
}

