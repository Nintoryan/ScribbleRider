using UnityEngine;
using YTaxi;

public class Spoiler : MonoBehaviour
{
    [SerializeField] private Car _car;
    [SerializeField] private float _spoilerForce;
    [SerializeField] private float _nonEnableReductionCoef;
    

    public bool Enable;
    
    private void Update()
    {
        if (Enable)
        {
            _car.Model.AddForce(-_car.Model.transform.up*_spoilerForce);
        }
        else
        {
            _car.Model.AddForce(-_car.Model.transform.up * (_spoilerForce * _nonEnableReductionCoef));
        }
        
    }
}
