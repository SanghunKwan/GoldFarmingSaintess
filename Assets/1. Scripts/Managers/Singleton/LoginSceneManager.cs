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
    RelayManager _relayManager;



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
    public string GetTicketId() => _loginManager._ticketId;
    public string GetPlayerId() => _loginManager._playerId;
    public void ShowLinkStatus(in MatchingStatusProtocol statusProtocol)
    {
        _loginManager.TransferMatchingStatus(statusProtocol);
    }
    public void NickNameDetermined(in UserSettingProtocol settingProtocol)
    {
        _loginManager.NickNameDetermined(settingProtocol);
    }
    public void GameStart(in GameStartProtocol startProtocol)
    {
        _loginManager.MatchingComplete(startProtocol);
        _loginManager.UpdateMatchingStatus(Time.time + 3);
        //_loginManager.ServerLinkEnd();

        StartCoroutine(GFSManager.WaitForSecond(3, () => SceneManager.LoadScene(1, LoadSceneMode.Single)));
    }
    public void SetHost(in SetHostProtocol protocol)
    {
        if (_relayManager == null)
            _relayManager = new RelayManager();

        AsyncSetHost(protocol);
    }
    async void AsyncSetHost(SetHostProtocol protocol)
    {
        _loginManager.HostingReady(await _relayManager.InitRelay(protocol._matchingSize), protocol._groupIndex);
        _relayManager.LinkRelay();
    }
    public void CancelHost()
    {
        _relayManager.CancelRelay();
    }
    #endregion ManagerTransfer
}
