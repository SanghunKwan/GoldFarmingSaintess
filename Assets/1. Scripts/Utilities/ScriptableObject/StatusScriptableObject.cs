using GFSUtilities.Unit;
using GFSUtilities;
using UnityEngine;

namespace GFSBattle
{
    [CreateAssetMenu(fileName = "StatScriptableObject", menuName = "Scriptable Objects/StatusScriptableObject")]
    public class StatusScriptableObject : ScriptableObject
    {
        public UnitTypes _type;
        public Status[] _stat;
    }
}
