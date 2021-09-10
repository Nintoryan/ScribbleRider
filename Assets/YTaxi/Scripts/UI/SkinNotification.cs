using UnityEngine;
using UnityEngine.UI;

public class SkinNotification : MonoBehaviour
{
    public static int NewSkins
    {
        get => PlayerPrefs.GetInt("YTaxi_New_Skins");
        set => PlayerPrefs.SetInt("YTaxi_New_Skins",value);
    }

    [SerializeField] private Text _amount;
    [SerializeField] private Image _star;

    private void Start()
    {
        _amount.gameObject.SetActive(NewSkins != 0);
        _star.gameObject.SetActive(NewSkins != 0);
    }
    
}
