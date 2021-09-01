using UnityEngine;

public class ObstaclesWater : MonoBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        var fo = other.GetComponent<FloatingObject>();
        if (fo != null)
        {
            fo.rb.AddForce(-Physics.gravity * fo.rb.mass*1.01f);
        }
    }
}
