using System.Collections;
using UnityEngine;
using YTaxi.Wheels;
using Random = UnityEngine.Random;

namespace YTaxi.Bot
{
    public class PrebuiltBotWheel : MonoBehaviour
    {
        [SerializeField] private Car _car;
        [SerializeField] private Car _player;
        
        [SerializeField] private Wheel[] _wheelsVariants;
    
        private bool _stop;
    
        private void Start()
        {
            _player.OnFirstWheelSet += () => { ApplyRandomWheel();};
            _car.OnFirstWheelSet += () => { StartCoroutine(RandomWheelSet()); };
            _car.OnFinished += () => { _stop = true; };
        }
    
        private void ApplyRandomWheel()
        {
            var wheel = Instantiate(_wheelsVariants[Random.Range(0, _wheelsVariants.Length - 1)]);
            _car.SetWheels(wheel);
        }
    
        private IEnumerator RandomWheelSet()
        {
            yield return new WaitForSeconds(5f);
            ApplyRandomWheel();
            if(!_stop)
                StartCoroutine(RandomWheelSet());
        }
    }
}

