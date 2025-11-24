using GFSManagers;
using GFSUtilities;
using GFSUtilities.Protocol;
using UnityEngine;
using UnityEngine.SceneManagement;


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
        GameManager.Instance.InstantiatePrefab(GFSUtilities.UI.UIType.PlayersUI, _noneBGManager.transform).GetComponent<PlayersUI>().InitUI(pageProtocol._nickNames);

        StartCoroutine(GFSManager.WaitForSecond(3, () => SceneManager.LoadScene(0, LoadSceneMode.Single)));
    }
    #endregion ManagerTransfer
}
