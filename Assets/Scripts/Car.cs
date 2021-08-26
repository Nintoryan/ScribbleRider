using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Car : MonoBehaviour
{
    [SerializeField] private List<Transform> _wheelPoints;
    [SerializeField] private Rigidbody _model;
    [SerializeField] private float _speed;
    [SerializeField] private List<GameObject> _currentWheels = new List<GameObject>();

    private void FixedUpdate()
    {
        _model.velocity = _model.transform.forward * _speed;
    }
    public void SetWheels(GameObject wheelExample, float Distance)
    {
        StartCoroutine(IESetWheels(wheelExample, Distance));
    }

    private IEnumerator IESetWheels(GameObject wheelExample, float Distance)
    {
        Debug.Log(Distance);
        //_model.transform.localPosition += new Vector3(0,1.5f,0);
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
