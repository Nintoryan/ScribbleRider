using System;
using UnityEngine;
using YTaxi.Zones;

namespace YTaxi.Bot
{
    public class BotSpeedModifyer : MonoBehaviour
    {
        [SerializeField] private Car _bot;
        [SerializeField] private Finish _finish;

        public AnimationCurve _duringDistanceSpeedModify;

        public float SpeedCoef;
            

        private float _trackDistance;
        private float _startXPosition;
        private void Start()
        {
            if (_finish == null)
            {
                _finish = FindObjectOfType<Finish>();
            }
            _trackDistance = _finish.transform.position.x - _bot.Model.transform.position.x;
            _startXPosition = _bot.Model.transform.position.x;
        }

        private void Update()
        {
            var progress = (_bot.Model.transform.position.x - _startXPosition) / _trackDistance;
            SpeedCoef = _duringDistanceSpeedModify.Evaluate(Mathf.Clamp(progress ,0,1));
            _bot.ApplyModifyedBaseSpeed(this);
        }
    }

}
