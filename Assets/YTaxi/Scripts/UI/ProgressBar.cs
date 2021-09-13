using UnityEngine;
using UnityEngine.UI;
using YTaxi.Data;

namespace YTaxi.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _progressfill;
        [SerializeField] private RectTransform _progressScale;
        
        [SerializeField] private Text _levelNumber;
        
    
        private float _start;
        private float _length;
        private float _currentLength;

    
        public void PassCurrent(float _currentX)
        {
            _currentLength = _currentX - _start;
            SetValue(_currentLength/_length);
        }
    
        public void Initialize(float _carStartX, float _finishX)
        {
            _start = _carStartX;
            _length = _finishX - _start;
    
            _levelNumber.text = PlayerData.LevelNumber.ToString();
        }
        
        private void SetValue(float value)
        {
            var clampedValue = Mathf.Clamp(value,0,1);
            _progressfill.fillAmount = clampedValue;
            _progressScale.localScale = new Vector3(clampedValue,1,1);
        }
    }
}

