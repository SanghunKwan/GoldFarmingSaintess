using GFSManagers;
using UnityEngine;

public class LoginWindow : BaseBGWindow<LoginWindow, LoginManager, LoginNoneBGManager>
{

    public override void InitWindow(LoginManager manager)
    {
        base.InitWindow(manager);

        gameObject.SetActive(false);
    }
    public override void FadeIn()
    {
        gameObject.SetActive(true);

    }

    public override void FadeOut()
    {
        throw new System.NotImplementedException();
    }

    #region Event
    public void OnClickLoginButton()
    {
        _manager.LinkServer();
    }
    #endregion Event
}
