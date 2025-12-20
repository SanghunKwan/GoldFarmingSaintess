using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.UI;
using UnityEngine;
using static DisturbSlot;

public class DisturbWindow : BaseBGWindow<DisturbWindow, DisturbManager, BGManager>
{
    [SerializeField] DisturbSlot[] _slots;

    bool[] _isPriced;
    string[] _saleStrings;
    Color[] _saleColors;


    public override void InitWindow(DisturbManager manager)
    {
        base.InitWindow(manager);

        _isPriced = new bool[_slots.Length];

        int length = (int)DisturbOptionType.Max;
        _saleStrings = new string[length];
        _saleColors = new Color[length];

        length = (int)DisturbOptionType.FirstFree;
        _saleStrings[length] = "무료";
        _saleColors[length] = GameManager.Instance._PlayerColorScriptableObject._color[(int)ColorType.Green];

        length = (int)DisturbOptionType.AfterPriced;
        //어떤 슬롯의 값을 가져와도 상관 없음.
        _saleStrings[length] = _slots[0]._GetDefaultButtonString;
        _saleColors[length] = _slots[0]._GetDefaultButtonColor;

        gameObject.SetActive(false);
    }
    public override void FadeIn()
    {
        gameObject.SetActive(true);
        _anim.SetTrigger(UIHashID.t_FadeIn);
    }
    void SetButton(DisturbType disturbType, DisturbOptionType saleOptionType)
    {
        int typeIndex = (int)saleOptionType;
        int slotIndex = (int)disturbType;

        _slots[slotIndex].SetText(TextType.Sale, _saleColors[typeIndex]);
        _slots[slotIndex].SetText(TextType.Sale, _saleStrings[typeIndex]);
    }
    void SetPrices(DisturbType type, int price)
        => _slots[(int)type].SetText(TextType.Price, price.ToString("N0"));
    void SetResult(DisturbType type, int num)
        => _slots[(int)type].SetText(TextType.Result, num.ToString());


    public override void FadeOut()
    {
        _anim.SetTrigger(UIHashID.t_FadeOut);
        _controller.HideAllColor(0.3f);
        StartCoroutine(GFSManager.WaitForSecond(0.4f, () => gameObject.SetActive(false)));
    }
    public void UpdatePrice(DisturbType type, int newPrice)
    {
        int index = (int)type;
        _slots[index].SetButtonInteractive(true);
        SetPrices(type, newPrice);
        if (!_isPriced[index])
        {
            SetButton(type, DisturbOptionType.AfterPriced);
            _isPriced[index] = true;
        }
    }
    public void UpdateResult(DisturbType type, int intensity)
        => SetResult(type, intensity);
    #region Event
    public void OnClickButtons(int index)
    {
        if (_manager.SelectDisturb((DisturbType)index))
            _slots[index].SetButtonInteractive(false);
    }
    public void OnClickSlots(int index)
    {
        _manager.CallExplain((DisturbType)index);
    }
    #endregion Event
}
