using MinigamesCommon;
using UnityEngine;

public class test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        new MiniGameInitializer().InitializeGame("YTaxi",20);
    }

}
