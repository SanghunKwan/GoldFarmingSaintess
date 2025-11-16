using UnityEngine;

[CreateAssetMenu(fileName = "SelectDataScriptableObject", menuName = "Scriptable Objects/SelectDataScriptableObject")]
public class SelectDataScriptableObject : ScriptableObject
{
    [Header("SelectOption")]
    public int _minCost;
    public int _maxCost;

    public int _battleCount;
    [Header("StartCountCost")]
    public int _advencedCost;
    public int _expertCost;
}
