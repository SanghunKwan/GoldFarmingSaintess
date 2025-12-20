using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ExplainWindow : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _title;
    [SerializeField] TextMeshProUGUI _detail;
    [SerializeField] TextMeshProUGUI _moreDetails;

    RectTransform _rectTransform;
    CanvasGroup _canvasGroup;


    [SerializeField] CanvasGroup _buttonsGroup;
    GameObject[] _buttons;

    readonly Vector2 _defaultSize = new Vector2(200, 70);

    public event Action _UseEvent;
    public event Action _ThrowEvent;


    public void InitWindow()
    {
        _rectTransform = (RectTransform)transform;
        _canvasGroup = GetComponent<CanvasGroup>();
        SetWindowActive(false);

        _buttons = new GameObject[_buttonsGroup.transform.childCount];
        for (int i = 0; i < _buttons.Length; i++)
        {
            _buttons[i] = _buttonsGroup.transform.GetChild(i).gameObject;
        }
    }
    public void SetWindowActive(bool isOn)
    {
        _canvasGroup.blocksRaycasts = isOn;
        _canvasGroup.alpha = isOn ? 1 : 0;
    }
    public void CallWindow(in Vector3 position)
    {
        _rectTransform.pivot
            = (position.x + _rectTransform.sizeDelta.x > Screen.width) ? Vector2.one : Vector2.up;


        SetWindowActive(true);
        transform.localPosition = position;

    }

    public void SetText(in string title, in string details)
    {
        _detail.SetText(details);
        _title.SetText(title);
    }
    public void SetMoreDetailText(in string details)
    {
        _moreDetails.SetText(details);
    }
    public void SetSize(bool isButtonOn)
    {
        float buttonHight;
        _buttonsGroup.blocksRaycasts = _buttonsGroup.interactable = isButtonOn;
        if (isButtonOn)
        {
            buttonHight = _defaultSize.y / 2;
            _buttonsGroup.alpha = 1;
        }
        else
            _buttonsGroup.alpha = buttonHight = 0;

        _rectTransform.sizeDelta = _defaultSize + Vector2.up * (_detail.preferredHeight
                                                                + _moreDetails.preferredHeight + buttonHight);

        Vector2 pivot = _rectTransform.pivot;
        pivot.y = (_rectTransform.sizeDelta.y - _rectTransform.anchoredPosition.y > Screen.height) ? 0 : 1;

        _rectTransform.pivot = pivot;
    }
    public void SetButtonSetActive(int index, bool isOn)
        => _buttons[index].SetActive(isOn);

    public void ClearEvent()
    {
        _ThrowEvent = null;
        _UseEvent = null;
    }

    #region Event
    public void OnClickWindow()
    {
        SetWindowActive(false);
    }
    public void OnClickUse()
    {
        _UseEvent?.Invoke();
        SetWindowActive(false);
    }
    public void OnClickThrowAway()
    {
        _ThrowEvent?.Invoke();
        SetWindowActive(false);
    }
    #endregion Event
}
