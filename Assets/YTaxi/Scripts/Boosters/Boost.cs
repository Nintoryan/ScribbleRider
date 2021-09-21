using System.Collections;
using UnityEngine;
using YTaxi.Scripts.Car;

namespace YTaxi.Scripts.Boosters
{
    public class Boost : MonoBehaviour
    {
        [SerializeField] private float _multibliedBy;
        [SerializeField] private float _duration;
        [SerializeField] private GameObject _graphics;

        private void OnTriggerEnter(Collider other)
        {
            var carEffects = other.GetComponentInParent<CarEffects>();
            if (carEffects != null && _graphics.activeInHierarchy)
            {
                carEffects.ModelSpeed *= _multibliedBy;
                carEffects.WheelSpeed *= _multibliedBy;
                StartCoroutine(EndBoostEffect(carEffects, _duration));
                _graphics.SetActive(false);
            }
        }

        private IEnumerator EndBoostEffect(CarEffects _carEffects,float duration)
        {
            yield return new WaitForSecondsRealtime(duration);
            _carEffects.ResetSpeed();
        }
    }
}

