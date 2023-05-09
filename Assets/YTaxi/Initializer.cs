using MinigamesCommon;
using UnityEngine;
using UnityEngine.SceneManagement;
using YTaxi.Scripts.Progress;

namespace YTaxi
{
    public class Initializer : MonoBehaviour
    {
        private void Start()
        {
            MiniGameScoreData.Last = 0;
            MiniGameScoreData.Session = 0;
            
            PlayerData.LevelNumber = MiniGameInitializer.StartLevel;
            MiniGameLevelsLoop.Initialize(10,PlayerData.AmountOfLevels);
            var levelToLoad = PlayerData.LevelNumber > PlayerData.AmountOfLevels ? MiniGameLevelsLoop.GetNextRandomLevel() : PlayerData.LevelNumber;

            PlayerData.CurrentSceneNumber = levelToLoad;
            SceneManager.LoadScene($"YTaxi/Scenes/Level{levelToLoad}");
        }

    }
}
