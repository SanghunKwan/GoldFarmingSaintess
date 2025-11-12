using GFSUtilities.Upgrade;
using UnityEngine;

public class GameManager : BaseDontDestoryManager<GameManager>
{
    public int[] _healUpgrade { get; private set; }


    public override void InitManager()
    {
        _healUpgrade = new int[(int)UpgradeType.Max];
    }
}
