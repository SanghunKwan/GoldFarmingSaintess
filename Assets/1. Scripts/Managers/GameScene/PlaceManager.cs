using GFSBattle;
using UnityEngine;

public class PlaceManager
{
    BaseUnit _unit;

    public void InitManager()
    {

    }

    public void DragInUnit(BaseUnit unit)
    {
        _unit = unit;

        _unit.SetGrab(true);
    }
    public void DragOutUnit(BaseUnit unit)
    {
        if (_unit != unit) return;

        _unit.SetGrab(false);

    }
}
