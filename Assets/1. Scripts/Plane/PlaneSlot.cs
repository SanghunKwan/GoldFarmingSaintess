using GFSBattle;
using GFSUtilities.Effect;
using UnityEngine;

public class PlaneSlot : MonoBehaviour
{
    [SerializeField] ParticleSystem _particleSystem;

    public BaseUnit _unit { get; private set; }

    public PlaneSlotEffectType _State { get; set; }



    public bool _IsPlayerUseless => _State == PlaneSlotEffectType.Useless || _State == PlaneSlotEffectType.EnemyOccupied;



    public void SetColor(in Color color)
    {
        var main = _particleSystem.main;

        main.startColor = color;
        _particleSystem.Stop(false, ParticleSystemStopBehavior.StopEmittingAndClear);
        _particleSystem.Play();
    }
    public void EnrollUnit(BaseUnit unit, PlaneSlotEffectType occupied)
    {
        _State = occupied;
        _unit = unit;
        _unit.transform.position = transform.position;
    }
    public void UnenrollsUnit(PlaneSlotEffectType usable)
    {
        _State = usable;
        _unit = null;
    }
}
