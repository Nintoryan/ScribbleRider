using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Car : MonoBehaviour
{
    [SerializeField] private List<Transform> _wheelPoints;
    [SerializeField] private Rigidbody _model;
    [SerializeField] private float _speed;
    public Rigidbody Model => _model;
    private float _baseSpeed;
    private float _nonlinearityСoeff;
    [SerializeField] private List<GameObject> _currentWheels = new List<GameObject>();

    private void Start()
    {
        _baseSpeed = _speed;
    }

    private void FixedUpdate()
    {
        foreach (var wheel in _currentWheels)
        {
            wheel.GetComponent<Rigidbody>().AddTorque(new Vector3(0,0,-1)*_speed);
        }
        var forward = _model.transform.forward;
        var Velocity = new Vector3(forward.x, Mathf.Clamp(forward.y,0,100000), forward.z);
        Velocity = Vector3.Lerp(Velocity,_model.transform.up,0.05f);
        _model.velocity = Velocity * _speed;
    }
    public void SetWheels(GameObject wheelExample, float Distance, float nonlinearityСoeff)
    {
        Debug.Log($"Nonlinnearity Coef of Drawing={nonlinearityСoeff}");
        _nonlinearityСoeff = nonlinearityСoeff;
        StartCoroutine(IESetWheels(wheelExample, Distance));
    }

    private IEnumerator IESetWheels(GameObject wheelExample, float Distance)
    {
        Debug.Log(Distance);
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

    public void ApplySlowEffect(float _speedReduceCoef)
    {
        _speed = Mathf.Lerp(_baseSpeed * _speedReduceCoef, 1, _nonlinearityСoeff / 100f);
    }

    public void DisposeSlowEffect()
    {
        _speed = _baseSpeed;
    }
}
