using GFSManagers;
using UnityEngine;


public class LoginSceneManager : MonoBehaviour
{
    public static LoginSceneManager Instance { get; private set; }


    [Header("¾À ³» ¸Å´ÏÀú")]
    [SerializeField] BGManager _bgManager;
    [SerializeField] LoginNoneBGManager _noneBGManager;

    LogoManager _logoManager;
    LoginManager _loginManager;


    private void Awake()
    {
        Instance = this;
        Application.runInBackground = true;
    }

    private void Start()
    {
        _bgManager.InitManager();
        ShowUnitychanLogo();
    }

    void ShowUnitychanLogo()
    {
        _logoManager = new LogoManager();
        _logoManager.InitManager(_bgManager);
        _logoManager.CallWindow();
    }

    public void LoginReady()
    {
        _loginManager = new LoginManager();
        _loginManager.InitManager(_noneBGManager);
        _loginManager.MakeWindow();
        _loginManager.CallWindow();
    }

}
