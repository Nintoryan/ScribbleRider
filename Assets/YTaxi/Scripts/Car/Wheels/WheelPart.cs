using UnityEngine;

namespace YTaxi.Wheels
{
    public class WheelPart : MonoBehaviour
    {
        [SerializeField] private MeshRenderer _meshRenderer;

        public void SetMaterial(Material material)
        {
            _meshRenderer.material = material;
        }
    }

}
