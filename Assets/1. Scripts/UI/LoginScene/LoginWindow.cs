using GFSManagers;
using GFSUtilities;
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

    public override void InitWindow(LoginManager manager)
    {
        base.InitWindow(manager);

        gameObject.SetActive(false);
        _pages[0].SetActive(true);
        for (int i = 1; i < _pages.Length; i++)
        {
            _pages[i].SetActive(false);
        }

    }
    public override void FadeIn()
    {
        gameObject.SetActive(true);
    }

    public override void FadeOut()
    {
        throw new System.NotImplementedException();
    }

    public void SetPage(int index, bool isOn)
        => _pages[index].SetActive(isOn);

    void SubmitNickname()
    {
        _manager.SubmitNickName(_nickNameInputField.text);
    }
    void SetMatching(bool isOn)
    {
        _anim.SetBool(UIHashID.b_IsMatching, isOn);
        _manager.SetMatching(isOn);
    }
    public void UpdateMatchingData(int num)
    {
        _matchingCountText.text = num.ToString();
    }
    #region Event
    public void OnClickLoginButton()
    {
        _manager.LinkServer();
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
