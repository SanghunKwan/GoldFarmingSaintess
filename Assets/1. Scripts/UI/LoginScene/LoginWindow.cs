using GFSManagers;
using UnityEngine;

public class LoginWindow : BaseBGWindow<LoginWindow, LoginManager, BGManager>
{

    public override void InitWindow(LoginManager manager)
    {
        base.InitWindow(manager);

    }
    public override void FadeIn()
    {
        throw new System.NotImplementedException();
    }

    public override void FadeOut()
    {
        throw new System.NotImplementedException();
    }

    #region Event
    public void OnClickLoginButton()
    {
        LoginSceneManager.Instance.Link();
    }
    #endregion Event
}
