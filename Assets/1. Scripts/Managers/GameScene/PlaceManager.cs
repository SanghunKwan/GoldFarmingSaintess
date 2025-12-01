using GFSBattle;
using System.Collections;
using UnityEngine;

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

        public void InitManager(PlaneManager planeManager)
        {
            _planeManager = planeManager;

            _cam = Camera.main;
            _dragMaxDistance = 10;
            _dropMaxDistance = 20;
            _dragLayerMask = 1 << LayerMask.NameToLayer("DragRaycast");
            _dropLayerMask = 1 << LayerMask.NameToLayer("Plane");
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

            Vector3 lastVec = Input.mousePosition + Input.mousePositionDelta;
            Ray ray = _cam.ScreenPointToRay(lastVec);
            Vector3 tempPosition;

            if (Physics.Raycast(ray, out RaycastHit hit, _dropMaxDistance, _dropLayerMask))
                _planeManager.CheckSlot(hit.point);

            if (Physics.Raycast(ray, out hit, _dragMaxDistance, _dragLayerMask))
                _unit.transform.position = hit.point;

            bool isOutofRange = false;

            yield return null;

            while (_isOnDrag)
            {
                tempPosition = Input.mousePosition;
                if (lastVec != tempPosition)
                {
                    ray = _cam.ScreenPointToRay(tempPosition);

                    if (Physics.Raycast(ray, out hit, _dragMaxDistance, _dragLayerMask))
                        _unit.transform.position = hit.point;

                    isOutofRange = !Physics.Raycast(ray, out hit, _dropMaxDistance, _dropLayerMask);
                    if (!isOutofRange)
                    {
                        //hit point에 이펙트 추가
                        //배치 가능 영역 추가.
                        //다른 캐릭터와 같은 자리에 배치 시 위치 교체.
                        lastVec = tempPosition;
                        _planeManager.CheckSlot(hit.point);
                    }
                    else
                    {
                        _planeManager.CheckSlot(beforePosition);
                    }
                }
                yield return null;
            }

            if (isOutofRange)
            {
                _unit.transform.position = beforePosition;
                _planeManager.SetFreeSlot();
            }
            else
                _planeManager.SelectSlot(_unit, beforePosition);

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
            if (_unit == null) return;

            SetGrabFalse();
        }
    }
}