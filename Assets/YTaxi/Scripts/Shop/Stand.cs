using DG.Tweening;
using UnityEngine;

namespace YTaxi.Scripts.Shop
{
    public class Stand : MonoBehaviour
    {
        [SerializeField] private Transform _stand;
        private void Start()
        {
            _stand.DOLocalRotate(new Vector3(0,360,0), 10f,RotateMode.FastBeyond360).SetLoops(-1).SetEase(Ease.Linear);
        }
    }
}

