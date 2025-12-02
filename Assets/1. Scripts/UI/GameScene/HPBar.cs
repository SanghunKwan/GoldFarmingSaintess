using GFSBattle;
using UnityEngine;
using UnityEngine.UI;


public class HPBar : MonoBehaviour
{
    [SerializeField] Slider _slider;
    [SerializeField] Image _fillImage;


    BaseUnit _unit;

    public void InitBar(Camera cam, BaseUnit unit, in Color color)
    {
        _unit = unit;

        int startHp = _unit._RefFullStat._hp;
        _slider.maxValue = startHp;
        _fillImage.color = color;

        _unit.OnHpChanged += SetValue;
        _unit.OnMoved += (vec) => FollowBar(vec, cam);

        SetValue(startHp);
        FollowBar(unit.transform.position, cam);
        _unit.EnrollDieEvent(Disactivate);
    }
    void SetValue(int value)
    {
        _slider.value = value;
    }
    void FollowBar(Vector3 vec, Camera cam)
    {
        Vector3 screen = cam.WorldToScreenPoint(vec);
        transform.position = screen;
    }
    public void Disactivate()
    {
        gameObject.SetActive(false);
        _unit = null;
    }
}
