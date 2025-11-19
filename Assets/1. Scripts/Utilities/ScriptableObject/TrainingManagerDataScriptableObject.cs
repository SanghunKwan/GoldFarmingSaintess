using UnityEngine;

[CreateAssetMenu(fileName = "TrainingManagerDataScriptableObject", menuName = "Scriptable Objects/TrainingManagerDataScriptableObject")]
public class TrainingManagerDataScriptableObject : ScriptableObject
{
    [Header("0:defaultHealAmount\n1:defaultHealCount")]
    public int[] defaultArray;
    public int[] defaultPrices;

    [Header("0:addHealAmount(%)\n1:addHealCount(%)")]
    public float[] addArray;

    [Space]
    public float exponent;
    [Space]
    public int _inventorySize;
}
