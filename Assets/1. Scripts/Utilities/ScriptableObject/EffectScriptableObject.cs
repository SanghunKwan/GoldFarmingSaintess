using GFSUtilities;
using GFSUtilities.Unit;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GFSBattle
{
    [CreateAssetMenu(fileName = "EffectScriptableObject", menuName = "Scriptable Objects/EffectScriptableObject")]
    public class EffectScriptableObject : ScriptableObject
    {
        [Header("force 관련 이펙트")]
        public GameObject[] healEffects;
        public GameObject[] buffEffects;

        [Header("force, star 관련 이펙트")]
        public ListCover[] baseEffects;

        [Header("force, type 관련 이펙트")]
        public ListCover[] weaponEffects;

        [Header("plane 관련 이펙트")]
        public GameObject[] planeEffects;

        [Header("UICam 관련 이펙트")]
        public GameObject[] camEffects;

        [Serializable]
        public class ListCover
        {
            public GameObject[] _list;
        }
    }
}

