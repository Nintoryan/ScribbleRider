using Plugins.Demigiant.DOTween.Modules;
using UnityEngine;

namespace YTaxi.Scripts.Shop
{
    public class Tabs : MonoBehaviour
    {
        [SerializeField] private RectTransform _container;
        [SerializeField] private float _step;
        [SerializeField] private Tab[] _tabses;
        
        
        private float _zeroPosition;

        private void Start()
        {
            _zeroPosition = _container.anchoredPosition.x;
            for (int i = 0; i < _tabses.Length; i++)
            {
                for (int j = 0; j < _tabses.Length; j++)
                {
                    if(i == j) continue;
                    _tabses[i].OnSelected += _tabses[j].Deselect;
                }
            }
            
        }

        public void OpenTab(int number)
        {
            _container.DOAnchorPosX(_zeroPosition + number*_step,0.5f);
        
        }
    }
}

