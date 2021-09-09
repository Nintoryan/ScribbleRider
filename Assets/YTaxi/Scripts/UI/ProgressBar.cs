using UnityEngine;
using UnityEngine.UI;
using YTaxi.Data;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Image _progressScaler;
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
        _progressScaler.fillAmount = value;
    }
}
