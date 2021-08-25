using UnityEngine;

public class TargetFollower : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _followingSpeed = 10;
    private Vector3 _offset;
    private Transform _transform;
 
    private void Awake()
    {
        _transform = GetComponent<Transform>();
        _offset = _transform.position - _target.position;
    }
 
    private void Update()
    {
        _transform.position = Vector3.MoveTowards(_transform.position, _target.position + _offset, Time.deltaTime * _followingSpeed);
    }
}