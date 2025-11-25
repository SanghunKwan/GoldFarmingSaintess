using Unity.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SceneChangeDataScriptableObject", menuName = "Scriptable Objects/SceneChangeDataScriptableObject")]
public class SceneChangeDataScriptableObject : ScriptableObject
{
    public string _names;
    public int _nameIndex;
    public int _size;
}
