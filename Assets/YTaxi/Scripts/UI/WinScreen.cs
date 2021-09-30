using DG.Tweening;
using MinigamesCommon;
using UnityEngine;
using UnityEngine.UI;
using YTaxi.Scripts.Progress;

namespace YTaxi.Scripts.UI
{
    public class WinScreen : MonoBehaviour
    {
        [SerializeField] private Text _score;
        [SerializeField] private int minScore;
        [SerializeField] private int maxScore;
        
        
        
        [SerializeField] private RectTransform _nextButtonRect;
        
        [SerializeField] private Image[] _nextButtonImages;
        [SerializeField] private Text _nextButtonText;
        
        
        [SerializeField] private RectTransform _winScreen;
        [SerializeField] private Image _blur;
        [SerializeField] private RectTransform _topBar;
        
        private void OnEnable()
        {
            var score = Random.Range(minScore, maxScore);
            
            MiniGameScoreData.Last = score;
            MiniGameScoreData.Session += score;
            
            new MiniGameCityMediator().SavePassedLevel("YTaxi",PlayerData.LevelNumber,MiniGameScoreData.Session);
            
            _score.text = $"+{score} очков";
            
            var s = DOTween.Sequence();
            s.SetAutoKill(true);
            s.Append(_blur.DOFade(0.8f, 1f));
            s.Join(_winScreen.DOAnchorPosY(0, 1f));
            s.Join(_topBar.DOAnchorPosY(-600, 1f));
            s.AppendInterval(2f);
            
            s.Append(_score.rectTransform.DOAnchorPosY(_score.rectTransform.position.y + 200f, 0.8f));
            s.Join(_score.DOFade(0, 0.4f));
            s.Join(_nextButtonRect.DOAnchorPosY(-93.6f, 0.8f));
            foreach (var image in _nextButtonImages)
            {
                s.Join(image.DOFade(1, 0.8f));
            }
    
            s.Join(_nextButtonText.DOFade(1, 0.8f));
            
        }
    }
}

