using GFSBattle;
using UnityEngine;

public class HPFollower : MonoBehaviour
{
    BaseUnit _unit;
    void Start()
    {
        _unit = GetComponent<BaseUnit>();
    }

    void Update()
    {
        Debug.Log(_unit._RefStat._hp);
    }
}
