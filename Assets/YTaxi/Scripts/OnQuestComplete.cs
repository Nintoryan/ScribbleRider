using MinigamesCommon;
using UnityEngine;
using UnityEngine.UI;
using YTaxi.Scripts.Progress;

namespace YTaxi.Scripts
{
    public class OnQuestComplete : MonoBehaviour
    {
        [SerializeField] private Text header;
        [SerializeField] private Text _body;
        [SerializeField] private Image _button;
        [SerializeField] private Text _buttonText;
        
        [SerializeField] private Sprite _newSprite;

        private void OnEnable()
        {
            var currentLevel = PlayerData.LevelNumber;
            if (MiniGameInitializer.StartLevel + MiniGameInitializer.LevelsToComplete <= currentLevel)
            {
                header.text = "Отлично";
                _body.text = "Квест выполнен!";
                _buttonText.text = "";
                _button.sprite = _newSprite;
            }
        }
    }
}
