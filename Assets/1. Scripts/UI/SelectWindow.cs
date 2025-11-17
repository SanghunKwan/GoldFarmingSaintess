using GFSManagers;
using GFSUtilities;
using GFSUtilities.UI;
using GFSUtilities.Unit;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectWindow : BaseBGWindow<SelectWindow, SelectManager>
{
    IReadOnlyList<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> _allyLists;
    IReadOnlyList<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> _enemyLists;
    IReadOnlyList<BattleCondition> _conditionList;

    [SerializeField] Transform _allyContentTr;
    [SerializeField] Transform _enemyContentTr;

    [SerializeField] TextMeshProUGUI _aidGold;
    [SerializeField] TextMeshProUGUI _resourceRate;

    int _currentIndex;

    Queue<UnitSelectSlot> _usingSlots;
    Queue<UnitSelectSlot> _unusingSlots;

    public override void InitWindow(SelectManager manager)
    {
        base.InitWindow(manager);
        _usingSlots = new Queue<UnitSelectSlot>();
        _unusingSlots = new Queue<UnitSelectSlot>();
    }

    public override void FadeIn()
    {
        throw new System.NotImplementedException();
    }

    public override void FadeOut()
    {
        gameObject.SetActive(false);
        _manager.EndSelect();
    }

    public void SetValue(IReadOnlyList<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> allyLists,
                         IReadOnlyList<IReadOnlyDictionary<KeyValuePair<StarCount, UnitTypes>, int>> enemyLists,
                         IReadOnlyList<BattleCondition> conditionList)
    {
        _allyLists = allyLists;
        _enemyLists = enemyLists;
        _conditionList = conditionList;

        _currentIndex = 0;

        PrintList(_currentIndex);
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
            UnitSelectSlot slot;

            if (_unusingSlots.Count > 0)
            {
                slot = _unusingSlots.Dequeue();
                slot.transform.SetParent(contentTr);
                slot.SetActive(true);
            }
            else
            {
                GameObject obj = GameManager.Instance.InstantiateResourcePrefab(UIResourceType.SelectSlot, contentTr);
                slot = obj.GetComponent<UnitSelectSlot>();
            }

            _usingSlots.Enqueue(slot);
            slot.SetStars(item.Key.Key);
            slot.SetImage(GameManager.Instance._UISpriteScriptableObject._sprites[(int)item.Key.Value - 1 + (isEnemyList ? (int)UnitTypes.Count : 0)]);
            slot.SetCount(item.Value);
        }
    }
    void ChagePage(int newPageIndex)
    {
        while (_usingSlots.Count > 0)
            SetDisableSlot(_usingSlots.Dequeue());

        PrintList(newPageIndex);
    }
    void SetDisableSlot(UnitSelectSlot slot)
    {
        _unusingSlots.Enqueue(slot);
        slot.SetActive(false);
    }
    void TurnPage(int changedPages)
    {
        _currentIndex = (_currentIndex + changedPages) % _conditionList.Count;
        ChagePage(_currentIndex);
    }
    void ChooseData()
    {
        _manager.SetBattleIndex(_currentIndex);
        FadeOut();
    }
    #region Event
    public void OnClickBeforeBattle()
    {
        TurnPage(-1);
    }
    public void OnClickNextBattle()
    {
        TurnPage(1);
    }
    public void OnClickChooseBattle()
    {
        ChooseData();
    }
    #endregion Event
}
