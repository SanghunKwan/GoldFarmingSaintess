using GFSManagers;
using GFSUtilities;
using GFSUtilities.UI;
using GFSUtilities.Unit;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectWindow : BaseBGWindow
{
    IReadOnlyList<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> _allyLists;
    IReadOnlyList<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> _enemyLists;
    IReadOnlyList<BattleCondition> _conditionList;

    [SerializeField] Transform _allyContentTr;
    [SerializeField] Transform _enemyContentTr;

    [SerializeField] TextMeshProUGUI _aidGold;
    [SerializeField] TextMeshProUGUI _resourceRate;



    public override void FadeIn()
    {
        throw new System.NotImplementedException();
    }

    public override void FadeOut()
    {
        throw new System.NotImplementedException();
    }

    public void SetValue(IReadOnlyList<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> allyLists,
                         IReadOnlyList<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> enemyLists,
                         IReadOnlyList<BattleCondition> conditionList)
    {
        _allyLists = allyLists;
        _enemyLists = enemyLists;
        _conditionList = conditionList;

        PrintList(0);
    }

    public void PrintList(int index)
    {
        IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> dic = _allyLists[index];

        CreateNSetSlots(dic, _allyContentTr, false);

        dic = _enemyLists[index];

        CreateNSetSlots(dic, _enemyContentTr, true);

        _resourceRate.text = _conditionList[index]._huntingRate.ToString("N");
        _aidGold.text = _conditionList[index]._participationAidGold.ToString("N0");
    }
    void CreateNSetSlots(IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int> dic, Transform contentTr, bool isEnemyList)
    {
        foreach (var item in dic)
        {
            GameObject obj = GameManager.Instance.InstantiateResourcePrefab(UIResourceType.SelectSlot, contentTr);
            UnitSelectSlot slot = obj.GetComponent<UnitSelectSlot>();

            slot.MakeStars(item.Key.Key);
            slot.SetImage(GameManager.Instance._UISpriteScriptableObject._sprites[(int)item.Key.Value - 1 + (isEnemyList ? (int)UnitTypes.Count : 0)]);
            slot.SetCount(item.Value);
        }

    }
}
