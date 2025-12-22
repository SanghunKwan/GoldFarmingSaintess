using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneChangeDataScriptableObject", menuName = "Scriptable Objects/SceneChangeDataScriptableObject")]
public class SceneChangeDataScriptableObject : ScriptableObject
{
    public string _names;
    public int _nameIndex;
    public int _size;
    public string _joinCode;

    public void CopyValue(SceneChangeDataScriptableObject copyObj)
    {
        _names = new string(copyObj._names);
        _nameIndex = copyObj._nameIndex;
        _size = copyObj._size;
        _joinCode = copyObj._joinCode;
    }
}
