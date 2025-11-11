using GFSUtilities;
using GFSUtilities.Unit;
using UnityEngine;

namespace GFSBattle
{
    [CreateAssetMenu(fileName = "EffectScriptableObject", menuName = "Scriptable Objects/EffectScriptableObject")]
    public class EffectScriptableObject : ScriptableObject
    {
        public Force _force;
        public GameObject[] effects;
        public float _statGrowthRate;
    }
}

