using GFSBattle;
using GFSUtilities.Unit;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace GFSManagers
{
    public class HPManager : MonoBehaviour
    {
        Dictionary<Force, UnitHpColor> _enumConverter;
        Camera _cam;

        Color[] _colors;
        GameManager _manager;
        Queue<HPBar> bars;


        public void InitManager()
        {
            _enumConverter = new Dictionary<Force, UnitHpColor>();
            _enumConverter.Add(Force.Enemy, UnitHpColor.Enemy);
            _enumConverter.Add(Force.Ally, UnitHpColor.Ally);

            _manager = GameManager.Instance;
            _colors = _manager._PlayerColorScriptableObject._color;

            _cam = Camera.main;

            bars = new Queue<HPBar>();
        }


        public void MakeHPBar(in LinkedList<BaseUnit> list, Force force)
        {
            Color hpbarColor = _colors[(int)_enumConverter[force]];

            foreach (BaseUnit unit in list)
            {
                GameObject go = _manager.InstantiateResourcePrefab(GFSUtilities.UI.UIResourceType.CharacterHPBar, transform);

                HPBar bar = go.GetComponent<HPBar>();
                bar.InitBar(_cam, unit, hpbarColor);
                bars.Enqueue(bar);
            }
        }

        public void BattleEnd()
        {
            //현재 활성화되어 있는 HP바를 모두 비활성화함.
            //캐릭터와 연동 해제.

            foreach (HPBar bar in bars)
                bar.Disactivate();
        }
    }
}