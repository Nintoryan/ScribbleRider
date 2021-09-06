using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using YTaxi.Data;

namespace YTaxi.Shop
{
    public class Skin : MonoBehaviour
    {
        [SerializeField] private Text _text;
        [SerializeField] protected int _id;
        [SerializeField] private int _cost;
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
                    break;
                case State.Opened:
                    _text.text = "";
                    break;
                case State.Selected:
                    _text.text = "Выбрано";
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
