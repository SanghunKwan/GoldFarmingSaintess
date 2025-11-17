using GFSManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GFSUtilities.UI;
using GFSUtilities.Unit;

public class UnitSelectSlot : MonoBehaviour
{
    [SerializeField] Image _portrait;
    [SerializeField] TextMeshProUGUI _unitCount;
    [SerializeField] Transform _starParentTr;


    public void SetImage(in Sprite image)
    {
        _portrait.sprite = image;
    }
    public void SetCount(int count)
    {
        _unitCount.text = count.ToString("N0");
    }
    public void SetStars(StarCount count)
    {
        for (int i = 0; i < _starParentTr.childCount; i++)
            _starParentTr.GetChild(i).gameObject.SetActive((int)count > i);
    }

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }
}
