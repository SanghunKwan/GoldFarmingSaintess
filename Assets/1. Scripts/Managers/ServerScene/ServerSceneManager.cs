using UnityEngine;

public class ServerSceneManager : MonoBehaviour
{
    void Start()
    {
        GetComponent<ServerManager>().ASDF();
    }
}
