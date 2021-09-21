using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using YTaxi.Scripts.Car;

namespace YTaxi.Scripts
{
    public class Finish : MonoBehaviour
    {
        /// <summary>
        /// Invokes on any car reach the finish 
        /// </summary>
        public event UnityAction<Car.Car> OnFinished;
        private void OnTriggerEnter(Collider other)
        {
            var carEffects = other.GetComponentInParent<CarEffects>();
            if (carEffects != null)
            {
                carEffects.ModelSpeed *= 0;
                carEffects.WheelSpeed *= 0;
                StartCoroutine(Finished(carEffects.Car));
                carEffects.Car.Finish();
            }
        }
    
        private IEnumerator Finished(Car.Car _car)
        {
            yield return new WaitForSeconds(0.5f);
            OnFinished?.Invoke(_car);
        }
    }
}

