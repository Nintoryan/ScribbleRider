using YTaxi.Data;

namespace YTaxi.Shop
{
    public class CarSkin : Skin, SelectableItem
    {
        private void Start()
        {
            Refresh();
        }
        public void Select()
        {
            PlayerData.SelectedCar = _id;
            LoadState(PlayerData.SelectedCar);
        }

        public void Refresh()
        {
            LoadState(PlayerData.SelectedCar);
        }
    }

}
