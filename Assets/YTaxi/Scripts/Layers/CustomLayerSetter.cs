#if UNITY_EDITOR
using UnityEngine;

namespace YTaxi.Layers
{
    [ExecuteInEditMode]
    public class CustomLayerSetter : MonoBehaviour
    {
        [SerializeField] private string layer;
        
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
