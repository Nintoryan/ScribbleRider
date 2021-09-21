using UnityEngine;

namespace YTaxi.Scripts.Car.Wheels
{
    public class WheelPart : MonoBehaviour
    {
        [SerializeField] private MeshRenderer[] _meshRenderers;

        public void SetMaterial(Material material)
        {
            foreach (var meshRenderer in _meshRenderers)
            {
                meshRenderer.material = material;
            }
        }
    }

}
