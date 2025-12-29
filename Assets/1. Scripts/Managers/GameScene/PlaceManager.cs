using GFSBattle;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GFSManagers
{
    public class PlaceManager
    {
        BaseUnit _unit;

        IEnumerator _grabIEnum;
        Camera _cam;

        PlaneManager _planeManager;

        float _dragMaxDistance;
        float _dropMaxDistance;
        int _dragLayerMask;
        int _dropLayerMask;

        bool _isOnDrag;
        public bool _enabled { get; private set; }

        public void InitManager(PlaneManager planeManager)
        {
            _planeManager = planeManager;

            _cam = Camera.main;
            _dragMaxDistance = 10;
            _dropMaxDistance = 20;
            _dragLayerMask = 1 << LayerMask.NameToLayer("DragRaycast");
            _dropLayerMask = 1 << LayerMask.NameToLayer("Plane");
        }
        public void ActivateManager()
        {
            _enabled = true;
        }
        public void DragInUnit(BaseUnit unit)
        {
            _unit = unit;

            _grabIEnum = GrabPosition();
            _isOnDrag = true;
            _unit.SetGrab(_isOnDrag, _grabIEnum);
        }
        IEnumerator GrabPosition()
        {
            Vector3 beforePosition = _unit.transform.position;

            Vector3 lastVec = Mouse.current.position.ReadValue() + Mouse.current.delta.ReadValue();
            Ray ray;
            Vector3 tempPosition;

            bool isOutofRange = false;

            int beforeIndex = _planeManager.GetIndex(beforePosition);
            int tempLastIndex = beforeIndex;
            int tempCurrentIndex = beforeIndex;
            RaycastHit hit;

            do
            {
                tempPosition = Mouse.current.position.ReadValue();
                if (lastVec != tempPosition)
                {
                    ray = _cam.ScreenPointToRay(tempPosition);

                    if (Physics.Raycast(ray, out hit, _dragMaxDistance, _dragLayerMask))
                        _unit.transform.position = hit.point;

                    isOutofRange = (!Physics.Raycast(ray, out hit, _dropMaxDistance, _dropLayerMask)) || _planeManager.IsSlotInvalid(hit.point, out tempCurrentIndex);

                    if (!isOutofRange)
                    {
                        if (tempCurrentIndex != tempLastIndex)
                        {
                            //hit point에 이펙트 추가
                            //배치 가능 영역 추가.
                            //다른 캐릭터와 같은 자리에 배치 시 위치 교체.
                            lastVec = tempPosition;

                            _planeManager.HighLightSlot(tempCurrentIndex, GFSUtilities.Effect.PlaneSlotEffectType.HighLight);

                            _planeManager.SetFreeSlot(tempLastIndex);

                            if (tempLastIndex == -1)
                                _planeManager.SetFreeSlot(beforeIndex);

                            tempLastIndex = tempCurrentIndex;
                        }
                    }
                    else
                    {
                        if (tempLastIndex == -1)
                        {
                            yield return null;
                            continue;
                        }

                        _planeManager.HighLightSlot(beforeIndex, GFSUtilities.Effect.PlaneSlotEffectType.HighLight);

                        _planeManager.SetFreeSlot(tempLastIndex);
                        tempLastIndex = -1;
                    }
                }
                yield return null;
            } while (_isOnDrag);

            if (isOutofRange)
            {
                _unit.transform.position = beforePosition;
                _planeManager.SetFreeSlot(tempCurrentIndex);
            }
            else
                _planeManager.SelectSlot(_unit, tempCurrentIndex, beforeIndex);

            _unit = null;
        }

        public void DragOutUnit(BaseUnit unit)
        {
            if (_unit != unit) return;

            SetGrabFalse();
        }
        void SetGrabFalse()
        {
            _isOnDrag = false;
            _unit.SetGrab(_isOnDrag, _grabIEnum);
            _grabIEnum = null;
        }
        public void EndPlacePhase()
        {
            _enabled = false;

            if (_unit == null) return;

            SetGrabFalse();
        }
    }
}