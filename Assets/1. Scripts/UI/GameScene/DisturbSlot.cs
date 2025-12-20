using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisturbSlot : MonoBehaviour
{
    public enum TextType
    {
        Result,
        Price,
        Sale
    }


    [SerializeField] TextMeshProUGUI[] _texts;
    [SerializeField] Button _button;


    public Color _GetDefaultButtonColor => _texts[(int)TextType.Sale].color;
    public string _GetDefaultButtonString => _texts[(int)TextType.Sale].text;


    public void SetText(TextType type, in string text)
        => _texts[(int)type].text = text;
    public void SetText(TextType type, in Color color)
        => _texts[(int)type].color = color;


    public void SetButtonInteractive(bool isOn)
    {
        if (_button.interactable == isOn) return;

        _button.interactable = isOn;
    }


}
