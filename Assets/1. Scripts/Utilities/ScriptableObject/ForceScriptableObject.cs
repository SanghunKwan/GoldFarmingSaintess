using GFSUtilities.Unit;
using UnityEngine;

[CreateAssetMenu(fileName = "ForceScriptableObject", menuName = "Scriptable Objects/ForceScriptableObject")]
public class ForceScriptableObject : ScriptableObject
{
    public Force _force;
    public float _statGrowthRate;
}
