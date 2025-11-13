using GFSBattle;
using GFSUtilities;
using UnityEngine;

[RequireComponent(typeof(BaseUnit))]
public class BattleEndMove : MonoBehaviour
{
    float _angularSpeed;
    static readonly Quaternion _lookDestination = Quaternion.LookRotation(Vector3.back);



    public void InitMove(float angularSpeed, float destroyTime)
    {
        _angularSpeed = angularSpeed;

        StartCoroutine(GFSManager.WaitForSecond(destroyTime, () => Destroy(this)));
    }

    private void Update()
    {
        if (transform.rotation == _lookDestination) enabled = false;

        transform.rotation
            = Quaternion.RotateTowards(transform.rotation, _lookDestination, _angularSpeed * Time.deltaTime);
    }
}
