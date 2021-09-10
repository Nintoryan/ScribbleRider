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

