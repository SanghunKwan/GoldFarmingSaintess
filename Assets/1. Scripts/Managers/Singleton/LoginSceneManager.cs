using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using Unity.Entities;
using Unity.Entities.Serialization;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoginSceneManager : MonoBehaviour
{
    public static LoginSceneManager Instance { get; private set; }


    [Header("¾À ³» ¸Å´ÏÀú")]
    [SerializeField] BGManager _bgManager;
    [SerializeField] LoginNoneBGManager _noneBGManager;
    [SerializeField]

    LogoManager _logoManager;
    LoginManager _loginManager;



    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _bgManager.InitManager();

        var data = GameManager.Instance._SceneChangeDataScriptableObject;

        if (data._nameIndex <= 0)
        {
            ShowUnitychanLogo();
        }
        else
        {
            string nickname = data._names.Split(' ')[data._nameIndex - 1];

            GameManager.Instance.ResetOverrideObject();
            LoginReady();
            _loginManager.AutoLogin(nickname);
        }
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



    #region ManagerTransfer
    public void ServerLinkSuccss(in PageProtocol pageProtocol)
    {
        _loginManager.ServerLinkSuccss(pageProtocol);
    }
    public void NickNameDetermined(in UserSettingProtocol settingProtocol)
    {
        _loginManager.NickNameDetermined(settingProtocol);
    }
    public void UpdateMatchingStatus(in MatchingStatusProtocol matchingStatusProtocol)
    {
        _loginManager.UpdateMatchingStatus(matchingStatusProtocol);
    }
    public void GameStart(in GameStartProtocol pageProtocol)
    {
        _loginManager.MatchingComplete(pageProtocol);

        StartCoroutine(GFSManager.WaitForSecond(3, () => SceneManager.LoadScene(1, LoadSceneMode.Single)));
    }
    #endregion ManagerTransfer
}
