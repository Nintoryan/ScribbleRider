using UnityEngine;

namespace MinigamesCommon
{
    public static class MiniGameScoreData
    {
        public static int Last
        {
            get => PlayerPrefs.GetInt("MiniGameLastLevelScore");
            set => PlayerPrefs.SetInt("MiniGameLastLevelScore", value);
        }

        public static int Session
        {
            get => PlayerPrefs.GetInt("MiniGameSessionScore");
            set => PlayerPrefs.SetInt("MiniGameSessionScore", value);
        }
    }
}