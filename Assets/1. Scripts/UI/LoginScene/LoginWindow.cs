using GFSManagers;
using GFSUtilities;
using GFSUtilities.UI;
using System.Collections;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class LoginWindow : BaseBGWindow<LoginWindow, LoginManager, LoginNoneBGManager>
{
    [SerializeField] GameObject[] _pages;
    [SerializeField] InputField _nickNameInputField;

    [SerializeField] TextMeshProUGUI _matchingCountText;
    [SerializeField] TextMeshProUGUI _matchingTimeText;

    IEnumerator _fadeInOrderHandle;

    float _timeCorrectionValue;

    private void Update()
    {
        int second = Mathf.CeilToInt(Time.time - _timeCorrectionValue);

        _matchingTimeText.text = (second / 60).ToString("D2") + ":" + (second % 60).ToString("D2");
    }



    public override void InitWindow(LoginManager manager)
    {
        base.InitWindow(manager);

        gameObject.SetActive(false);
        _pages[0].SetActive(true);
        for (int i = 1; i < _pages.Length; i++)
            _pages[i].SetActive(false);

        _controller.HideAllColor(0);
        enabled = false;
    }
    public override void FadeIn()
    {
        gameObject.SetActive(true);
    }

    public override void FadeOut()
    {
        _anim.SetTrigger(UIHashID.t_Matched);
    }


    public void SetPage(int index, bool isOn)
    {
        _pages[index].SetActive(isOn);
    }

    void SubmitNickname()
    {
        _manager.SubmitNickName(_nickNameInputField.text);
    }
    public void FadeInOrder(LogingraphicGroupType type)
    {
        _controller.FadeGraphicInOrder((int)type, 1, 0.4f, -0.2f);
    }
    void SetMatching(bool isOn)
    {
        int index = (int)LogingraphicGroupType.MatchingTexts;

        _anim.SetBool(UIHashID.b_IsMatching, isOn);
        _manager.SetMatching(isOn);

        if (isOn)
        {
            _controller.FadeGraphicAtOnce(index, 0, 0f);
            StartCoroutine(GFSManager.WaitForSecond(0.25f, () =>
            {
                _fadeInOrderHandle = _controller.FadeGraphicInOrder(index, 1, 0.3f, -0.15f);
            }));
        }
        else
        {
            if (_fadeInOrderHandle != null)
                StopCoroutine(_fadeInOrderHandle);

            _controller.FadeGraphicAtOnce(index, 0, 0.1f);
            enabled = false;
        }
    }
    public void UpdateMatchingData(float endTime)
    {
        enabled = true;

        StartCoroutine(ShowCountDown(endTime));
    }
    IEnumerator ShowCountDown(float endTime)
    {
        while (true)
        {
            int num = Mathf.CeilToInt(endTime - Time.time);
            _matchingCountText.text = num.ToString();

            yield return null;
        }

    }

    #region Event
    public void OnClickLoginButton()
    {
        _manager.LinkServer();
        _manager.Authentication();
    }
    public void OnSubmitNickName()
    {
        SubmitNickname();
    }
    public void OnClickNickNameSubmit()
    {
        SubmitNickname();
    }
    public void OnClickMatchingButton()
    {
        SetMatching(true);
    }
    public void OnClickMatchingStopButton()
    {
        SetMatching(false);
    }
    public void OnClickSettingButton()
    {
        _manager.CallSettingWindow();
    }
    public void OnClickQuitButton()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion Event
}
