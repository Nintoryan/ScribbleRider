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
            SceneManager.LoadScene($"YTaxi/Scenes/Level{PlayerData.LevelNumber}");
        }

    }
}
