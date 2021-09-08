using UnityEngine;

public class WheelPart : MonoBehaviour
{
    [SerializeField] private MeshRenderer _meshRenderer;

    public void SetMaterial(Material material)
    {
        _meshRenderer.material = material;
    }
}
