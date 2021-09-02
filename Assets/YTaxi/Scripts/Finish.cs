using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using YTaxi;

public class Finish : MonoBehaviour
{
    public event UnityAction OnFinished; 
    private void OnTriggerEnter(Collider other)
    {
        var carEffects = other.GetComponentInParent<CarEffects>();
        if (carEffects != null)
        {
            carEffects.ModelSpeed *= 0;
            carEffects.WheelSpeed *= 0;
            StartCoroutine(Finished());

        }
    }

    private IEnumerator Finished()
    {
        yield return new WaitForSeconds(1f);
        OnFinished?.Invoke();
    }
}
