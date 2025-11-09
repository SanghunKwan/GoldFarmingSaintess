using GFSUtilities;
using GFSUtilities.Unit;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectScriptableObject", menuName = "Scriptable Objects/EffectScriptableObject")]
public class EffectScriptableObject : ScriptableObject
{
    public Force _force;
    public GameObject[] effects;
}
