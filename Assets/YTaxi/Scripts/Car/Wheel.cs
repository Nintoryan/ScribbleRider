using UnityEngine;

public class Wheel : MonoBehaviour
{
    public float _nonlinnearCoef;
    public int _amountOfSharpAngles;
    public float _distance;
    

    public void Initialize(float nonlinnearCoef, int amountOfSharpAngles, float distance)
    {
        _nonlinnearCoef = nonlinnearCoef;
        _amountOfSharpAngles = amountOfSharpAngles;
        _distance = distance;
    }
}
