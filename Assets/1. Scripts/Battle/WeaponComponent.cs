using GFSUtilities;
using UnityEngine;

public class WeaponComponent : MonoBehaviour
{
    //근접, 원거리에 따라 다름

    //근접일 때는 공격 시 이펙트만 있음.
    //원거리일 때는 투사체 생성.

    GameObject _effect;

    public void InitWeapon(GameObject effect)
    {
        _effect = effect;
        HideWeapon(false);
    }

    public void ActiveEffect()
    {
        GameObject tempEffect = Instantiate(_effect, transform);
        Destroy(tempEffect, 1);

        StartCoroutine(GFSManager.WaitForSecond(2, () => { }));
    }

    public void HideWeapon(bool isOn)
    {

    }
}
