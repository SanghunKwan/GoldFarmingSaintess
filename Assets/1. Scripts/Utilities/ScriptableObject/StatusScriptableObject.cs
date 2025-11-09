using GFSUtilities.Unit;
using UnityEngine;

namespace GFSUtilities
{
    [CreateAssetMenu(fileName = "StatScriptableObject", menuName = "Scriptable Objects/StatusScriptableObject")]
    public class StatusScriptableObject : ScriptableObject
    {
        public UnitTypes _type;
        public Status[] _stat;
    }
}
