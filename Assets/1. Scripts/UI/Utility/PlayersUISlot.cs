using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayersUISlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nickName;
    [SerializeField] TextMeshProUGUI _goldText;
    [SerializeField] Image _colorImage;


    [SerializeField] GameObject dataObject;

    public void InitSlot(in string nickName, in Color color)
    {
        _colorImage.color = color;
        _nickName.text = nickName;
    }
    public void SetShowData(bool isOn)
    {
        dataObject.SetActive(isOn);
    }

    public void ShowMoney(int gold)
    {
        _goldText.text = gold.ToString("N0");
    }

}
