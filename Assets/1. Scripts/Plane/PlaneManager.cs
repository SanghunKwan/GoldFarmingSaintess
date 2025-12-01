using GFSBattle;
using GFSUtilities.Effect;
using UnityEngine;


namespace GFSManagers
{
    public class PlaneManager : MonoBehaviour
    {
        public enum SlotStateType
        {
            UpDown = 0
        }


        GameObject _prefabSlotEffect;

        PlaneSlot[] _slots;

        Vector3 _startPosition;
        int _lastCheckedIndex;


        Color[] _colors;
        public int _row { get; private set; }
        public int _column { get; private set; }


        public int _SlotCount => _row * _column;



        public void InitManager()
        {
            _prefabSlotEffect = GameManager.Instance._EffectScriptableObject.planeEffects[(int)PlaneResourceType.PlaneSlot];

            _row = Mathf.RoundToInt(transform.localScale.x * 10);
            _column = Mathf.RoundToInt(transform.localScale.z * 10);
            _startPosition = new Vector3(-(_row - 1f) / 2, 0, -(_column - 1f) / 2);

            _colors = GameManager.Instance._PlayerColorScriptableObject._color;
            _lastCheckedIndex = -1;

            ResetSlots();
            SetSlotsState(SlotStateType.UpDown);
        }

        public void ResetSlots()
        {
            int beforeCount = (_slots != null) ? _slots.Length : 0;

            System.Array.Resize(ref _slots, _SlotCount);

            Vector3 calculatePos = Vector3.zero;

            for (int i = 0; i < _slots.Length; i++)
            {
                calculatePos.x = i % _row;
                calculatePos.z = i / _column;

                if (i < beforeCount)
                    _slots[i].transform.position = _startPosition + calculatePos;
                else
                    _slots[i] = Instantiate(_prefabSlotEffect, _startPosition + calculatePos, Quaternion.identity, transform).GetComponent<PlaneSlot>();
            }
        }
        public void SetSlotsState(SlotStateType type)
        {
            switch (type)
            {
                case SlotStateType.UpDown:
                    for (int i = 0; i < _slots.Length; i++)
                    {
                        _slots[i]._State = (i >= (_column / 2 * _row)) ? PlaneSlotEffectType.Useless : PlaneSlotEffectType.Usable;
                        _slots[i].SetColor(_colors[(int)_slots[i]._State]);
                    }
                    break;
            }
        }

        public void CheckSlot(in Vector3 rayPoint)
        {
            HighLightSlot(GetIndex(rayPoint), PlaneSlotEffectType.HighLight);
        }
        public void HighLightSlot(int slotIndex, PlaneSlotEffectType highLight)
        {
            if (_lastCheckedIndex == slotIndex) return;

            if (_lastCheckedIndex >= 0)
                _slots[_lastCheckedIndex].SetColor(_colors[(int)_slots[_lastCheckedIndex]._State]);

            if (_slots[slotIndex]._IsPlayerUseless)
            {
                _lastCheckedIndex = -1;
                return;
            }

            _slots[slotIndex].SetColor(_colors[(int)highLight]);

            _lastCheckedIndex = slotIndex;
        }
        public void SetFreeSlot()
        {
            if (_lastCheckedIndex < 0) return;

            _slots[_lastCheckedIndex].SetColor(_colors[(int)_slots[_lastCheckedIndex]._State]);
        }
        public void SelectSlot(BaseUnit unit, in Vector3 beforePosition)
        {
            if (_lastCheckedIndex < 0) return;

            int beforeIndex = GetIndex(beforePosition);

            SwapSlot(unit, _lastCheckedIndex, beforeIndex, PlaneSlotEffectType.Occupied, PlaneSlotEffectType.Usable);
            _lastCheckedIndex = -1;
        }
        public void SelectSlot(BaseUnit unit, int index, PlaneSlotEffectType occupied)
        {
            _slots[index].EnrollUnit(unit, occupied);
            _slots[index].SetColor(_colors[(int)occupied]);
        }
        public void SwapSlot(BaseUnit unit, int nextIndex, int beforeIndex, PlaneSlotEffectType occupied, PlaneSlotEffectType usable)
        {
            if (_slots[nextIndex]._State == occupied)
                _slots[beforeIndex].EnrollUnit(_slots[nextIndex]._unit, occupied);

            else if (beforeIndex != nextIndex && _slots[nextIndex]._State == usable)
            {
                _slots[beforeIndex].UnenrollsUnit(usable);
                _slots[beforeIndex].SetColor(_colors[(int)usable]);
            }

            SelectSlot(unit, nextIndex, occupied);
        }



        int GetIndex(in Vector3 rayPoint)
        {
            Vector3 offset = rayPoint - _startPosition;

            int rowIndex = Mathf.Min(Mathf.FloorToInt(offset.x + 0.5f), _row - 1);
            int columnIndex = Mathf.Min(Mathf.FloorToInt(offset.z + 0.5f), _column - 1);
            return rowIndex + columnIndex * _row;
        }
    }
}