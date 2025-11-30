using GFSUtilities.Effect;
using UnityEngine;


namespace GFSManagers
{
    public class PlaneManager : MonoBehaviour
    {

        GameObject _prefabSlotEffect;

        PlaneSlot[] _slots;


        public int _SlotCount => _row * _column;
        public int _row { get; private set; }
        public int _column { get; private set; }


        public void InitManager()
        {
            _prefabSlotEffect = GameManager.Instance._EffectScriptableObject.planeEffects[(int)PlaneResourceType.PlaneSlot];

            _row = Mathf.RoundToInt(transform.localScale.x * 10);
            _column = Mathf.RoundToInt(transform.localScale.z * 10);

            ResetSlots();
        }

        public void ResetSlots()
        {
            int beforeCount = (_slots != null) ? _slots.Length : 0;

            System.Array.Resize(ref _slots, _SlotCount);

            Vector3 startPosition = new Vector3(-(_row - 1f) / 2, 0, -(_column - 1f) / 2);
            Vector3 calculatePos = Vector3.zero;

            for (int i = 0; i < beforeCount; i++)
            {
                calculatePos.x = i % _row;
                calculatePos.z = i / _column;

                _slots[i].transform.position = startPosition + calculatePos;
            }
            for (int i = beforeCount; i < _slots.Length; i++)
            {
                calculatePos.x = i % _row;
                calculatePos.z = i / _column;

                _slots[i] = Instantiate(_prefabSlotEffect, startPosition + calculatePos, Quaternion.identity, transform).AddComponent<PlaneSlot>();
            }
        }

        public void CheckSlot()
        {

        }


    }
}