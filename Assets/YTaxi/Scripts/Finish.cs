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

        private bool isBotFirst;
        private bool isWin => PlayerXPos - transform.position.x > BotXPos - transform.position.x;

        private void OnTriggerEnter(Collider other)
        {
            var carEffects = other.GetComponentInParent<CarEffects>();
            
            if (carEffects != null)
            {
                if (carEffects.Car == CoreLinks.Instance.Bot)
                {
                    isBotFirst = true;
                }
                carEffects.ModelSpeed *= 0;
                carEffects.WheelSpeed *= 0;
                carEffects.Car.Finish();
                
                if (carEffects.Car == CoreLinks.Instance.Player)
                {
                    StartCoroutine(EndLevel(isWin && !isBotFirst));
                }

                var a = new GameObject[12];
                var b = a.Any(c => c.activeSelf);
            }
        }
    
        private IEnumerator EndLevel(bool win)
        {
            yield return new WaitForSeconds(0.2f);
            OnFinished?.Invoke(win);
        }
    }
}

