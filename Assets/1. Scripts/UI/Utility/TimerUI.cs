using GFSManagers;
using GFSUtilities;
using GFSUtilities.UI;
using System;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _textCount;
    Animator _anim;

    int _lastTime;
    float _warnLeftTime;

    public Color _defaultColor;
    Color _warnColor;

    bool _isTimerWarned;

    public void InitUI(in Color warnColor)
    {
        _anim = GetComponent<Animator>();

        GameManager manager = GameManager.Instance;
        _warnLeftTime = manager._TurnScriptableObject._warnLeftTime;
        _warnColor = warnColor;
        _defaultColor = _textCount.color;
        _isTimerWarned = false;
    }
    public void ResetTimer()
    {
        _textCount.color = _defaultColor;
        _isTimerWarned = false;
    }

    public void SetCount(float time)
    {
        if (time >= _warnLeftTime)
            ShowIntTime(time);
        else
            ShowFloatTime(time);
    }
    void ShowIntTime(float time)
    {
        int tempTime = Mathf.FloorToInt(time);

        if (_lastTime == tempTime) return;
        _lastTime = tempTime;
        _textCount.text = _lastTime.ToString();
    }
    void ShowFloatTime(float time)
    {
        if (!_isTimerWarned)
        {
            _isTimerWarned = true;
            _textCount.color = _warnColor;
        }

        _textCount.text = time.ToString("N1");
    }

    public void FadeIn(in Action fadeInAction)
    {
        _anim.SetTrigger(UIHashID.t_FadeIn);
        DelayTimerAction(0.3f, fadeInAction);
    }
    public void FadeOut()
    {
        _anim.SetTrigger(UIHashID.t_FadeOut);
    }
    public void Shake(in Action shakeAction)
    {
        _anim.SetTrigger(UIHashID.t_Shake);
        DelayTimerAction(0.15f, shakeAction);
    }
    void DelayTimerAction(float delayTime, in Action action)
        => StartCoroutine(GFSManager.WaitForSecond(delayTime, action));

}
