using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Car : MonoBehaviour
{
    [SerializeField] private List<Transform> _wheelPoints;
    [SerializeField] private Rigidbody _model;
    [SerializeField] private float _wheelsSpeed;
    [SerializeField] private float _modelSpeed;

    public float WheelSpeed
    {
        get => _wheelsSpeed;
        set => _wheelsSpeed = value;
    }

    public float ModelSpeed
    {
        get => _modelSpeed;
        set => _modelSpeed = value;
    }

    public Rigidbody Model => _model;
    public float BaseWheelSpeed { get; private set; }
    public float BaseModelSpeed { get; private set; }
    public float NonlinearityСoeff { get; private set; }
    
    public int AmountOfSharpAngles { get; private set; }

    private List<GameObject> _currentWheels = new List<GameObject>();

    private void Start()
    {
        BaseWheelSpeed = _wheelsSpeed;
        BaseModelSpeed = _modelSpeed;
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < _currentWheels.Count; i++)
        {
            for (int j = 0; j < _currentWheels[i].transform.childCount-1; j++)
            {
                Gizmos.DrawSphere(_currentWheels[i].transform.GetChild(j).position,0.01f);
                Gizmos.DrawLine(_currentWheels[i].transform.GetChild(j).position,
                    _currentWheels[i].transform.GetChild(j+1).position);
            }
        }
    }

    private void FixedUpdate()
    {
        foreach (var wheel in _currentWheels)
        {
            wheel.GetComponent<Rigidbody>().AddTorque(new Vector3(0,0,-1)*_wheelsSpeed);
        }
        var forward = _model.transform.forward;
        var Velocity = new Vector3(forward.x, Mathf.Clamp(forward.y,0,100000), forward.z);
        Velocity = Vector3.Lerp(Velocity,_model.transform.up,0.05f);
        if(!Mathf.Approximately(_modelSpeed,0))
            _model.velocity = Velocity * _modelSpeed;
    }
    public void SetWheels(GameObject wheelExample, float Distance, float nonlinearityСoeff, int amountOfSharpAngles)
    {
        // Debug.Log($"Nonlinnearity Coef of Drawing={nonlinearityСoeff}");
        // Debug.Log($"Kiki Coef of Drawing={kikiCoef}");
        NonlinearityСoeff = nonlinearityСoeff;
        AmountOfSharpAngles = amountOfSharpAngles;
        StartCoroutine(IESetWheels(wheelExample, Distance));
    }

    private IEnumerator IESetWheels(GameObject wheelExample, float Distance)
    {
        foreach (var wheel in _currentWheels)
        {
            Destroy(wheel);
        }
        
        _currentWheels.Clear();
        
        yield return new WaitForEndOfFrame();
        for (int i = 0; i < _wheelPoints.Count; i++)
        {
            for (int j = 0; j < _wheelPoints[i].childCount; j++)
            {
                Destroy(_wheelPoints[i].GetChild(i).gameObject);
            }
            var wheel = Instantiate(wheelExample, transform);
            wheel.transform.position = _wheelPoints[i].position;
            wheel.transform.localScale *= 0.005f;
            wheel.transform.SetParent(transform);
            var joint = wheel.AddComponent<HingeJoint>();
            joint.connectedBody = _model;
            joint.axis = new Vector3(0,0,-1);
            joint.connectedMassScale = 1000f;
            
            _currentWheels.Add(wheel);
        }
        Destroy(wheelExample);
        _model.isKinematic = false;
    }
}
