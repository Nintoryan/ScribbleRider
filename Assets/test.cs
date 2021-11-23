using MinigamesCommon;
using UnityEngine;
using YTaxi.Scripts.Progress;

public class test : MonoBehaviour
{
    [SerializeField] private int _levelNumber;
    
    private void Start()
    {
        new MiniGameInitializer().InitializeGame("YTaxi",PlayerData.LevelNumber);
            //PlayerData.LevelNumber
    }

}
