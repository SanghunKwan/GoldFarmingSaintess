using UnityEngine;

[CreateAssetMenu(fileName = "SettlementScriptableObject", menuName = "Scriptable Objects/SettlementScriptableObject")]
public class SettlementScriptableObject : ScriptableObject
{
    [Header("정산")]
    public int _healFee;
    public float _deadReductionRate;
    public float _specialConditionRate;

    [Header("방해")]
    public int _disturbComprehensiveFee;
    public int _disturbPlayerFee;
    public int[] _disturbIntensity;

}
