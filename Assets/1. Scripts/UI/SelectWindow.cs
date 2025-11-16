using GFSUtilities.Unit;
using System.Collections.Generic;
using UnityEngine;

public class SelectWindow : BaseBGWindow
{
    public override void FadeIn()
    {
        throw new System.NotImplementedException();
    }

    public override void FadeOut()
    {
        throw new System.NotImplementedException();
    }

    public void SetValue(List<List<KeyValuePair<StarCount, UnitTypes>>> allyLists,
                         List<List<KeyValuePair<StarCount, UnitTypes>>> enemyLists)
    {
        for (int i = 0; i < allyLists.Count; i++)
        {
            for (int j = 0; j < allyLists[i].Count; j++)
            {
                Debug.Log(allyLists[i][j].Key + " : " + allyLists[i][j].Value);
            }
        }

        for (int i = 0; i < enemyLists.Count; i++)
        {
            for (int j = 0; j < enemyLists[i].Count; j++)
            {
                Debug.Log(enemyLists[i][j].Key + " : " + enemyLists[i][j].Value);
            }
        }
    }
}
