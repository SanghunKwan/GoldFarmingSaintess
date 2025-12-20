using GFSBattle;
using GFSManagers;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] Image _fillImage;

    BaseUnit _unit;
    HPManager _manager;

    public void InitBar(BaseUnit unit, in Color color, HPManager manager)
    {
        _unit = unit;
        _manager = manager;

        int startHp = _unit._RefFullStat._hp;
        _slider.maxValue = startHp;
        _fillImage.color = color;

        _unit.OnHpChanged += SetValue;
        _unit.OnMoved += FollowBar;

        SetValue(startHp);
        FollowBar(unit.transform.position);
        _unit.EnrollDieEvent(Disactivate);

    }
    void SetValue(int value)
    {
        _slider.value = value;
    }
    void FollowBar(Vector3 vec)
    {
        transform.localPosition = _manager.UIFollowWorld(vec);
    }
    public void Disactivate()
    {
        gameObject.SetActive(false);
        _unit = null;
    }
}
