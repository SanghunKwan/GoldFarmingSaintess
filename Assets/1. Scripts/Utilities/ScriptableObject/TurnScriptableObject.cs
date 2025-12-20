using UnityEngine;

[CreateAssetMenu(fileName = "TurnScriptableObject", menuName = "Scriptable Objects/TurnScriptableObject")]
public class TurnScriptableObject : ScriptableObject
{
    public int _maxTurn;
    public float _eventFailAdventage;
    public float _warnLeftTime;

    public float _voteOpenDelay;
    public float _prizeMoveTime;
    public int _voteSecondPrize;

    [Header("턴 시간(sec) : 턴, 이벤트,선택, 전투배치, 전투, 정산, 경매, 결산")]
    public float[] _times;
}
