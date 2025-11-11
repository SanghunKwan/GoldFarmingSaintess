using GFSBattle;
using UnityEngine;

[RequireComponent(typeof(BaseUnit))]
public class BattleEndMove : MonoBehaviour
{
    float _angularSpeed;
    static readonly Quaternion _lookDestination = Quaternion.LookRotation(Vector3.back);



    public void InitMove(float angularSpeed, float destroyTime)
    {
        _angularSpeed = angularSpeed;

        Destroy(this, destroyTime);
    }

    private void Update()
    {
        transform.rotation
            = Quaternion.RotateTowards(transform.rotation, _lookDestination, _angularSpeed * Time.deltaTime);

    }

}
