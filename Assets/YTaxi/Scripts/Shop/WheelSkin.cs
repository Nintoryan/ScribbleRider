using YTaxi.Data;

namespace YTaxi.Shop
{
    public class WheelSkin : Skin, SelectableItem
    {
        private void Start()
        {
            Refresh();
        }
        public void Select()
        {
            PlayerData.SelectedWheels = _id;
            LoadState(PlayerData.SelectedWheels);
        }

        public void Refresh()
        {
            LoadState(PlayerData.SelectedWheels);
        }
    }

}
