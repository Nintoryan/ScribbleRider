using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YTaxi.Data;

namespace YTaxi.Shop
{
    public class Skin : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] private Image _SelectedCheck;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _selected;
        [SerializeField] private Sprite _select;
        [SerializeField] private Sprite _locked;
        [SerializeField] private Button _button;

        [SerializeField] protected int _id;
        [SerializeField] private int _cost;
        public bool isAvailiable => _cost <= PlayerData.LevelNumber;
        private State _state;
        public int ID => _id;
        public event UnityAction<Skin> Selected;
        protected void LoadState(int _selectedID)
        {
            if (PlayerData.LevelNumber >= _cost)
            {
                _state = State.Opened;
            }

            if (_selectedID == _id)
            {
                _state = State.Selected;
                Selected?.Invoke(this);
            }
            RefreshView();
        }
        
        private void RefreshView()
        {
            switch (_state)
            {
                case State.Locked:
                    _text.text = $"{_cost} уровень";
                    _image.sprite = _locked;
                    _button.interactable = false;
                    _SelectedCheck.gameObject.SetActive(false);
                    break;
                case State.Opened:
                    _text.text = "";
                    _image.sprite = _select;
                    _button.interactable = true;
                    _SelectedCheck.gameObject.SetActive(false);
                    break;
                case State.Selected:
                    _text.text = "";
                    _image.sprite = _selected;
                    _button.interactable = true;
                    _SelectedCheck.gameObject.SetActive(true);
                    break;
            }
        }
    }

    public enum State
    {
        Locked,
        Opened,
        Selected
    }
}

public interface SelectableItem
{
    void Select();
    void Refresh();
}
