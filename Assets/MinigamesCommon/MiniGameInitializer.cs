using UnityEngine.SceneManagement;

namespace MinigamesCommon
{
    public class MiniGameInitializer
    {
        public static int StartLevel;
        public void InitializeGame(string gameName, int startLevel)
        {
            StartLevel = startLevel;
            SceneManager.LoadScene($"{gameName}/Scenes/InitialScene");
        }
        
    }
} 
