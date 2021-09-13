using System;
using System.Collections;
using UnityEngine;
using YTaxi.Wheels;

namespace YTaxi.Bot
{
    public class PrebuiltBotWheel : MonoBehaviour
    {
        [SerializeField] private Car _car;
        [SerializeField] private Car _player;
        
        [SerializeField] private WheelAndTime[] _wheelsVariants;
    
        private bool _stop;
        private int id;
    
        private void Start()
        {
            _player.OnFirstWheelSet += () => { StartCoroutine(StartWheelQueue()); };
            _car.OnFinished += () => { _stop = true; };
        }
    
        private void ApplyNextWheel()
        {
            var wheel = Instantiate(_wheelsVariants[id]._wheelVariant);
            _car.SetWheels(wheel);
        }
    
        private IEnumerator StartWheelQueue()
        {
            ApplyNextWheel();
            yield return new WaitForSeconds(_wheelsVariants[id].duration);
            id++;
            if(!_stop && id < _wheelsVariants.Length)
                StartCoroutine(StartWheelQueue());
        }
    }

    [Serializable]
    public class WheelAndTime
    {
        public Wheel _wheelVariant;
        public float duration;
    }
}

