using GFSManagers;
using GFSUtilities;
using GFSUtilities.Upgrade;
using TMPro;
using UnityEngine;

public class TrainingWindow : BaseBGWindow<TrainingWindow, TrainingManager, NoneBGManager>
{
    [SerializeField] TextMeshProUGUI[] _currentUpgradeLv;
    [SerializeField] TextMeshProUGUI[] _nextUpgradePrice;


    public override void InitWindow(TrainingManager manager)
    {
        base.InitWindow(manager);

        GetAllData();
        gameObject.SetActive(false);
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
    public void OnClickSlot(int index)
    {
        _manager.ShowExplain((UpgradeType)index);
    }
    #endregion Event

}
