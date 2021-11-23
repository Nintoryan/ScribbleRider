using UnityEngine.SceneManagement;

namespace MinigamesCommon
{
    public class MiniGameInitializer
    {
        public static int StartLevel{ get; private set; }
        public static int LevelsToComplete { get; private set; }

        public void InitializeGame(string gameName, int startLevel,int levelsToComplete)
        {
            StartLevel = startLevel;
            LevelsToComplete = levelsToComplete;
            SceneManager.LoadScene($"{gameName}/Scenes/InitialScene");
        }
        
    }
} 
