using UnityEngine;
using YTaxi.Scripts.Car.Skins;
using YTaxi.Scripts.Progress;

namespace YTaxi.Scripts.Shop
{
    public class CarView : MonoBehaviour
    {
        [SerializeField] private SkinData _data;
        [SerializeField] private MeshRenderer[] _wheelParts;
        [SerializeField] private GameObject _currentModel;

        public Material GetCurrentMaterial()
        {
            return _data.GetWheelMaterial(PlayerData.SelectedWheels);
        }
        
        private void Start()
        {
            ApplyCar(PlayerData.SelectedCar);
        }

        public void ApplyWheel(int id)
        {
            var mat = _data.GetWheelMaterial(id);
            foreach (var wheelPart in _wheelParts)
            {
                wheelPart.material = mat;
            }
        }

        public void ApplyCar(int id)
        {
            var newModel = Instantiate(_data.GetCarModel(id), _currentModel.transform.parent);
            Destroy(_currentModel);
            _currentModel = newModel;
        }
        
    }

}
