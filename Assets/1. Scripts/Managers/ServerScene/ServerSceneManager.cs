using Unity.Collections;
using UnityEngine;

public class ServerSceneManager : MonoBehaviour
{
    public static ServerSceneManager Instance { get; private set; }

    ServerManager _serverManager;


    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        _serverManager = GetComponent<ServerManager>();
        _serverManager.InitManager();
    }

    public void NewClient(in FixedString64Bytes playerId)
    {
        _serverManager.TicketEnter(playerId.ToString());
    }
    public void ExitClient(in FixedString64Bytes playerId, in FixedString64Bytes ticketId)
    {
        _serverManager.TicketExit(playerId.ToString(), ticketId.ToString());
    }
}
