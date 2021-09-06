using DG.Tweening;
using UnityEngine;

public class Tabs : MonoBehaviour
{
    [SerializeField] private RectTransform _container;
    [SerializeField] private float _step;
    
    private float _zeroPosition;

    private void Start()
    {
        _zeroPosition = _container.anchoredPosition.x;
    }

    public void OpenTab(int number)
    {
        _container.DOAnchorPosX(_zeroPosition + number*_step,0.5f);
    }
}
