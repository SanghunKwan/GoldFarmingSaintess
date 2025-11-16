using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabScriptableObject", menuName = "Scriptable Objects/PrefabScriptableObject")]
public class PrefabScriptableObject : ScriptableObject
{
    public GameObject[] _prefabs;
}
