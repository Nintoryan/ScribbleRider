using System;
using UnityEngine;
using YTaxi;

public class Spoiler : MonoBehaviour
{
    [SerializeField] private Car _car;
    [SerializeField] private float _spoilerForce;
    [SerializeField] private float _baseReductionCoef;

    public float _Coef
    {
        get;
        private set;
    }

    private void Start()
    {
        _Coef = _baseReductionCoef;
    }

    public void Disable()
    {
        _Coef = 0;
    }

    public void Reset()
    {
        _Coef = _baseReductionCoef;
    }

    public void FullPower()
    {
        _Coef = 1;
    }

    private void Update()
    {
        _car.Model.AddForce(-_car.Model.transform.up*(_spoilerForce*_Coef));
    }
}
