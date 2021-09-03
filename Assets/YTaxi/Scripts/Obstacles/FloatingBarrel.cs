using DG.Tweening;
using UnityEngine;

public class FloatingBarrel : MonoBehaviour
{
    [SerializeField] private float _amplitude;

    private void Start()
    {
        var position = transform.position;
        
        var uppoint = position.y;
        var downpoint = position.y - 2*_amplitude;
        transform.position = new Vector3(position.x,downpoint,position.z);
        
        var s = DOTween.Sequence();
        s.Append(transform.DOMoveY(uppoint, 1f).SetEase(Ease.Linear));
        s.Append(transform.DOMoveY(downpoint, 1f).SetEase(Ease.Linear));
        s.SetLoops(-1);
    }
}
