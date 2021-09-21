using UnityEngine;

namespace YTaxi.Scripts.Progress
{
    public static class PlayerData
    {
        public const int AmountOfLevels = 7;
        public static int LevelNumber
        {
            get => PlayerPrefs.GetInt("YTaxi_Level_Number");
            set => PlayerPrefs.SetInt("YTaxi_Level_Number", value);
        }
        public static int SelectedWheels
        {
            get => PlayerPrefs.GetInt("YTaxi_Selected_Wheels");
            set => PlayerPrefs.SetInt("YTaxi_Selected_Wheels", value);
        }
        public static int SelectedCar
        {
            get => PlayerPrefs.GetInt("YTaxi_Selected_Car");
            set => PlayerPrefs.SetInt("YTaxi_Selected_Car", value);
        }
    }
}

