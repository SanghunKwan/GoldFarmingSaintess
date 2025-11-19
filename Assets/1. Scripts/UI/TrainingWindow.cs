using GFSManagers;
using GFSUtilities;
using GFSUtilities.Item;
using GFSUtilities.UI;
using GFSUtilities.Upgrade;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TrainingWindow : BaseBGWindow<TrainingWindow, TrainingManager, NoneBGManager>
{
    const string imageName = "ItemImage";
    [SerializeField] Transform _inventorySlotTr;
    [SerializeField] TextMeshProUGUI[] _currentUpgradeLv;
    [SerializeField] TextMeshProUGUI[] _nextUpgradePrice;


    Image[] itemSlots;


    public override void InitWindow(TrainingManager manager)
    {
        base.InitWindow(manager);

        gameObject.SetActive(false);
        Item[] items = _manager._items;

        itemSlots = new Image[items.Length];
        for (int i = 0; i < itemSlots.Length; i++)
        {
            GameObject go = GameManager.Instance.InstantiateResourcePrefab(UIResourceType.TrainingSlot, _inventorySlotTr);
            itemSlots[i] = go.transform.Find(imageName).GetComponent<Image>();
        }

        GetAllData();
    }



    public override void FadeIn()
    {
        gameObject.SetActive(true);
        _anim.SetTrigger(UIHashID.t_FadeIn);
    }

    public override void FadeOut()
    {
        _anim.SetTrigger(UIHashID.t_FadeOut);
        StartCoroutine(GFSManager.WaitForSecond(0.5f, () => gameObject.SetActive(false)));
    }

    public void GetAllData()
    {
        for (UpgradeType type = UpgradeType.HealAmount; type < UpgradeType.Max; type++)
            GetData(type);
    }
    public void GetData(UpgradeType type)
    {
        int index = (int)type;
        _currentUpgradeLv[index].text = (_manager._currentUpgrades[index] + 1).ToString();
        _nextUpgradePrice[index].text = _manager._NextUpgradePrice(type).ToString();
    }
    void TryUpgrade(UpgradeType type)
    {
        _manager.TryUpgrade(type);
    }
    #region Event
    public void OnClickUpgradeButton(int index)
    {
        TryUpgrade((UpgradeType)index);
    }

    #endregion Event

}
