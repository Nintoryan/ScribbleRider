using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace YTaxi.Scripts.Shop
{
    public class Tab : MonoBehaviour
    {
        [SerializeField] private Image _bg;
        [SerializeField] private Text _amountOfAvailiable;
        [SerializeField] private Skin[] _skins;
        [SerializeField] private Color _selectedBgColor;

        public event UnityAction OnSelected; 
    
        private void Start()
        {
            _amountOfAvailiable.text = _skins.Count(s => s.isAvailiable).ToString();
        }

        public void Select()
        {
            _bg.color = _selectedBgColor;
            OnSelected?.Invoke();
        }

        public void Deselect()
        {
            _bg.color = Color.clear;
        }
    }
}

