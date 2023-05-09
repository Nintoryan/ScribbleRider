using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using YTaxi.Scripts.Car;

namespace YTaxi.Scripts
{
    public class Finish : MonoBehaviour
    {
        private float PlayerXPos => CoreLinks.Instance.Player.Model.transform.position.x;
        private float BotXPos => CoreLinks.Instance.Bot.Model.transform.position.x;
        public event UnityAction<bool> OnFinished;

        private bool isFinished = false;

        private void Start()
        {
            isFinished = false;
        }

        private bool isWin => PlayerXPos - transform.position.x > BotXPos - transform.position.x;

        private void OnTriggerEnter(Collider other)
        {
            var carEffects = other.GetComponentInParent<CarEffects>();
            
            if (carEffects != null)
            {
                if (carEffects.Car == CoreLinks.Instance.Bot)
                {
                    StartCoroutine(EndLevel(false));
                }
                if (carEffects.Car == CoreLinks.Instance.Player)
                {
                    StartCoroutine(EndLevel(true));
                }

            }
        }
    
        private IEnumerator EndLevel(bool win)
        {
            if(isFinished)
                yield break;
            isFinished = true;
            yield return new WaitForSeconds(0.2f);
            CoreLinks.Instance.Player.Stop();
            CoreLinks.Instance.Bot.Stop();
            OnFinished?.Invoke(win);
        }
    }
}

