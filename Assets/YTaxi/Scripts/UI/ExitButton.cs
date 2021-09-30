using MinigamesCommon;
using UnityEngine;
using YTaxi.Scripts.Progress;

namespace YTaxi.Scripts.UI
{
    public class ExitButton : MonoBehaviour
    {
        public void Exit()
        {
            new MiniGameCityMediator().ExitMiniGame("YTaxi",PlayerData.LevelNumber,MiniGameScoreData.Session);
        }
    }

}

