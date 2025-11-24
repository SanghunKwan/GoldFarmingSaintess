using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayersUISlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nickName;
    [SerializeField] Image _colorImage;

    public void SetData(in string nickName, in Color color)
    {
        _colorImage.color = color;
        _nickName.text = nickName;
    }

}
