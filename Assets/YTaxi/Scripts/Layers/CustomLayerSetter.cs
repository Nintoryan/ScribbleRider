#if UNITY_EDITOR
using UnityEngine;

namespace YTaxi.Layers
{
    [ExecuteInEditMode]
    public class CustomLayerSetter : MonoBehaviour
    {
        private string layer = "Wheels";
        
        private void OnEnable()
        {
            Apply();
        }
        
        private void Apply()
        {
            gameObject.layer = LayerMask.NameToLayer(layer);
        }
        
    }  
}
#endif
