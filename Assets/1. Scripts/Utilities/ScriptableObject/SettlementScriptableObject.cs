using UnityEngine;

[CreateAssetMenu(fileName = "SettlementScriptableObject", menuName = "Scriptable Objects/SettlementScriptableObject")]
public class SettlementScriptableObject : ScriptableObject
{
    public int _healFee;
    public float _deadReductionRate;
    public float _specialConditionRate;

}
