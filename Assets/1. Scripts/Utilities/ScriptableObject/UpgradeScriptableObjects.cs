using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeScriptableObjects", menuName = "Scriptable Objects/UpgradeScriptableObjects")]
public class UpgradeScriptableObjects : ScriptableObject
{
    [Header("0:defaultHealAmount\n1:defaultHealCount")]
    public int[] defaultArray;

    [Header("0:addHealAmount(%)\n1:addHealCount(%)")]
    public float[] addArray;
}
