using System.Linq;
using UnityEngine;

namespace YTaxi.Shop
{
    public class Shop : MonoBehaviour
    {
        [SerializeField] private CarSkin[] _carSkins;
        [SerializeField] private WheelSkin[] _wheelSkins;

#if UNITY_EDITOR
        private void OnValidate()
        {
            _carSkins = FindObjectsOfType<CarSkin>();
            _wheelSkins = FindObjectsOfType<WheelSkin>();
        }
#endif
        private void Start()
        {
            foreach (var carSkin in _carSkins)
            {
                carSkin.Selected += RefreshCarSkins;
            }

            foreach (var wheelSkin in _wheelSkins)
            {
                wheelSkin.Selected += RefreshWheelSkins;
            }
        }

        private void RefreshCarSkins(Skin selected)
        {
            foreach (var skin in _carSkins.Where(c=>c!=selected))
            {
                skin.Refresh();
            }
        }
        private void RefreshWheelSkins(Skin selected)
        {
            foreach (var skin in _wheelSkins.Where(c=>c!=selected))
            {
                skin.Refresh();
            }
        }

    }
}

