using Unity.Collections;
using UnityEngine;

public class ServerSceneManager : MonoBehaviour
{
    public static ServerSceneManager Instance { get; private set; }

    ServerManager _serverManager;


    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;
        _serverManager = GetComponent<ServerManager>();
        _serverManager.InitManager();
    }

    public bool NewClient(in FixedString64Bytes playerId)
    {
        return _serverManager.TicketEnter(playerId.ToString());
    }
    public void ExitClient(in FixedString64Bytes playerId)
    {
        _serverManager.TicketExit(playerId.ToString());
    }
}
