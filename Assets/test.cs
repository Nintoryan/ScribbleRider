using MinigamesCommon;
using UnityEngine;
using YTaxi.Scripts.Progress;

public class test : MonoBehaviour
{
    [SerializeField] private int _levelNumber;
    [SerializeField] private int _levelsToComplete;
    
    
    private void Start()
    {
        new MiniGameInitializer().InitializeGame("YTaxi",PlayerData.LevelNumber,_levelsToComplete);
            //PlayerData.LevelNumber
    }

}
