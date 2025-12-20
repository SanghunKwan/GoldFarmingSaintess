using GFSBattle;
using GFSUtilities;
using GFSUtilities.Unit;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Device;


namespace GFSManagers
{
    public class HPManager : MonoBehaviour
    {
        Dictionary<Force, UnitHpColor> _enumConverter;
        Camera _cam;

        Color[] _colors;
        GameManager _manager;
        Queue<HPBar> bars;
        RectTransform _canvasTransform;

        public void InitManager()
        {
            _enumConverter = new Dictionary<Force, UnitHpColor>();
            _enumConverter.Add(Force.Enemy, UnitHpColor.Enemy);
            _enumConverter.Add(Force.Ally, UnitHpColor.Ally);

            _manager = GameManager.Instance;
            _colors = _manager._PlayerColorScriptableObject._color;

            _cam = Camera.main;
            _canvasTransform = (RectTransform)transform.parent;
            bars = new Queue<HPBar>();
        }


        public void MakeHPBar(LinkedList<BaseUnit> list, Force force)
        {
            Color hpbarColor = _colors[(int)_enumConverter[force]];

            StartCoroutine(GFSManager.WaitForSecond(0.5f, () => InstantiateBarsInList(list, hpbarColor)));
        }
        void InstantiateBarsInList(in LinkedList<BaseUnit> list, in Color hpbarColor)
        {
            foreach (BaseUnit unit in list)
            {
                GameObject go = _manager.InstantiateResourcePrefab(GFSUtilities.UI.UIResourceType.CharacterHPBar, transform);

                HPBar bar = go.GetComponent<HPBar>();
                bar.InitBar(unit, hpbarColor, this);
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

        public Vector2 UIFollowWorld(in Vector3 vec)
        {
            Vector3 screen = _cam.WorldToScreenPoint(vec);
            return UIPosition(screen);
        }
        public Vector3 WorldFollowUI(in Vector3 vec)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(_cam, vec);
            return _cam.ScreenToWorldPoint((Vector3)screenPoint + Vector3.forward);
        }
        public Vector2 UIPosition(in Vector3 vec)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, vec, _cam, out Vector2 localPoint);

            return localPoint;
        }
    }
}