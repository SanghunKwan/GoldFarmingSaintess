using UnityEngine;

[CreateAssetMenu(fileName = "ServerDataScriptableObject", menuName = "Scriptable Objects/ServerDataScriptableObject")]
public class ServerDataScriptableObject : ScriptableObject
{
    public string _ipv4;
    public ushort _port;

    public int _matchingCount;

    public void CopyValue(ServerDataScriptableObject copyObject)
    {
        _ipv4 = new string(copyObject._ipv4);
        _port = copyObject._port;

        _matchingCount = copyObject._matchingCount;
    }
}
