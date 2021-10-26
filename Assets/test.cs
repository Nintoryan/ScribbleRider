using MinigamesCommon;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private int _levelNumber;
    
    private void Start()
    {
        new MiniGameInitializer().InitializeGame("YTaxi",_levelNumber);
    }

}
