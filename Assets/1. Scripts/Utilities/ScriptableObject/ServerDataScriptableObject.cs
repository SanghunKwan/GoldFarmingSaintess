using UnityEngine;

[CreateAssetMenu(fileName = "ServerDataScriptableObject", menuName = "Scriptable Objects/ServerDataScriptableObject")]
public class ServerDataScriptableObject : ScriptableObject
{
    public string _ipv4;
    public ushort _port;

    public int _matchingCount;
}
