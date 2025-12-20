using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GFSUtilities.Unit;
using System;
using UnityEngine.EventSystems;

public class UnitSelectSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Image _portrait;
    [SerializeField] TextMeshProUGUI _unitCount;
    [SerializeField] Transform _starParentTr;
    [SerializeField] GraphicColorController _controller;

    public event Action OnclickEvent;

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

    public void FadeIn()
    {
        _controller.HideAllColor(0);
        _controller.FadeGraphicInOrder(0, 1, 0.2f, 0);
    }

    #region Event
    public void OnClickSlot()
    {
        OnclickEvent?.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickSlot();
    }
    #endregion Event
}
