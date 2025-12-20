using GFSUtilities;
using UnityEngine;

[CreateAssetMenu(fileName = "UnitStatScriptableObject", menuName = "Scriptable Objects/UnitStatScriptableObject")]
public class UnitStatScriptableObject : ScriptableObject
{
    public UnitData[] stat;
}
