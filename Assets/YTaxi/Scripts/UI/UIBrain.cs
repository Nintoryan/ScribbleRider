using MinigamesCommon;
using UnityEngine;
using UnityEngine.SceneManagement;
using YTaxi.Scripts.Progress;

namespace YTaxi.Scripts.UI
{
    public class UIBrain : MonoBehaviour
    {
        [SerializeField] private ProgressBar _progress;
        [SerializeField] private Car.Car _player;
        [SerializeField] private Finish _finish;
        [SerializeField] private GameObject _winCanvas;
        [SerializeField] private GameObject _looseCanvas;
        [SerializeField] private GameObject _drawingCanvas;
        [SerializeField] private GameObject _gamePlayCanvas;
        [SerializeField] private GameObject _menuCanvas;
        
        
    
        private void Start()
        {
            _progress.Initialize(_player.Model.transform.position.x,_finish.transform.position.x);
            _player.OnFirstWheelSet += () =>
            {
                _menuCanvas.SetActive(false);
            };
            _player.OnOutOfBounds += () =>
            {
                Time.timeScale = 0;
                OpenLooseCanvas();
            };
            _finish.OnFinished += DetectReachingFinish;
        }
    #if UNITY_EDITOR
        private void OnValidate()
        {
            if (_player == null)
            {
                _player = FindObjectOfType<Car.Car>();
            }
    
            if (_finish == null)
            {
                _finish = FindObjectOfType<Finish>();
            }
            
        }
    #endif
    
        private void DetectReachingFinish(bool isWin)
        {
            if (isWin)
            {
                OpenWinCanvas();
            }
            else
            {
                OpenLooseCanvas();               
            }
        }
        
        private void OpenWinCanvas()
        {
            _drawingCanvas.SetActive(false);
            _winCanvas.SetActive(true);
        }
    
        private void OpenLooseCanvas()
        {
            _gamePlayCanvas.SetActive(false);
            _drawingCanvas.SetActive(false);
            _looseCanvas.SetActive(true);
        }
    
        public void OpenShop()
        {
            SkinNotification.NewSkins = 0;
            SceneManager.LoadScene("YTaxi/Scenes/Shop");
        }
        
        private void Update()
        {
            _progress.PassCurrent(_player.Model.transform.position.x);
        }
    
        public void NextLevel()
        {
            if(PlayerData.LevelNumber % 5 == 0 && PlayerData.LevelNumber < PlayerData.AmountOfLevels)
                SkinNotification.NewSkins+=1;
            var levelToLoad = PlayerData.LevelNumber;
            if (levelToLoad > PlayerData.AmountOfLevels)
            {
                levelToLoad = MiniGameLevelsLoop.GetNextRandomLevel();
            }
            PlayerData.CurrentSceneNumber = levelToLoad;
            SceneManager.LoadScene($"YTaxi/Scenes/Level{levelToLoad}");
        }
    
        public void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    
        private void OnEnable()
        {
            Time.timeScale = 1.5f;
        }
    
        private void OnDisable()
        {
            Time.timeScale = 1f;
        }
    }
}

