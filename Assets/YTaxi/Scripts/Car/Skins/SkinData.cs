using UnityEngine;

namespace YTaxi.Scripts.Car.Skins
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SkinData", order = 1)]
    public class SkinData : ScriptableObject
    {
        [SerializeField] private Material[] _wheelMaterials;
        [SerializeField] private GameObject[] _carSkins;

        public Material GetWheelMaterial(int id)
        {
            return _wheelMaterials[id];
        }

        public GameObject GetCarModel(int id)
        {
            return _carSkins[id];
        }
    }
}

