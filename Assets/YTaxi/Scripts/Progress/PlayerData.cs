using UnityEngine;

namespace YTaxi.Data
{
    public static class PlayerData
    {
        public static int LevelNumber
        {
            get => PlayerPrefs.GetInt("YTaxi_Level_Number");
            set => PlayerPrefs.SetInt("YTaxi_Level_Number", value);
        }
    }
}

