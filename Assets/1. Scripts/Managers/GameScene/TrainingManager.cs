using GFSUtilities.Item;
using GFSUtilities.Upgrade;
using UnityEngine;
using GFSUtilities.UI;
using System.Text;


namespace GFSManagers
{
    public class TrainingManager : BaseBGWindowManager<TrainingWindow, TrainingManager, NoneBGManager>
    {
        int[] _defaultArray;
        int[] _defaultPriceArray;
        float[] _addArray;

        float _exponent;

        int _healFee;

        public int[] _currentUpgrades { get; private set; }
        public Item[] _items { get; private set; }
        public int _CurrentValue(UpgradeType type) => GetCurrentUpgradeValue((int)type);
        public int _NextUpgradePrice(UpgradeType type) => GetPrice((int)type);

        public ExplainManager _explainManager { get; set; }

        int GetCurrentUpgradeValue(int index) => Mathf.CeilToInt(_defaultArray[index] * (1f + (_addArray[index] * _currentUpgrades[index])));
        int GetPrice(int index) => Mathf.CeilToInt(_defaultPriceArray[index] * Mathf.Pow(_exponent, _currentUpgrades[index]));



        public override void InitManager(NoneBGManager _bgManager)
        {
            base.InitManager(_bgManager);

            GameManager manager = GameManager.Instance;

            TrainingManagerDataScriptableObject data = manager._TrainingDataScriptableObject;

            _defaultArray = data.defaultArray;
            _addArray = data.addArray;
            _defaultPriceArray = data.defaultPrices;
            _exponent = data.exponent;

            _currentUpgrades = new int[(int)UpgradeType.Max];
            _items = new Item[data._inventorySize];

            ReadyToTraining();

            _healFee = manager._SettlementScriptableObject._healFee;
        }
        public void ReadyToTraining()
        {
            GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Training, _bgManager.transform);
            _window = go.GetComponent<TrainingWindow>();
            _window.InitWindow(this);
            //_bgManager.CallUI(0.5f, _window);
        }
        public void ShowWindow()
        {
            _window.FadeIn();
        }

        public void TryUpgrade(UpgradeType type)
        {
            int needMoney = _NextUpgradePrice(type);

            if (!_bgManager.TryChangeGold(-needMoney)) return;

            _currentUpgrades[(int)type] += 1;
            _window.GetData(type);
        }

        public void TrainingTimeOut()
        {
            _window.FadeOut();
        }

        public void ShowExplain(UpgradeType type)
        {
            _explainManager.ShowAtMousPosition(ExplainType.Upgrade,
                (type == UpgradeType.HealAmount) ? GetHealAmountDetail() : GetHealCountDetail());
        }

        string GetHealAmountDetail()
        {
            StringBuilder sb = new StringBuilder();
            int index = (int)UpgradeType.HealAmount;
            sb.Append("현재 힐 양 : ");
            sb.AppendLine(GetCurrentUpgradeValue(index).ToString());

            sb.Append("업그레이드 증가량 : ");
            sb.AppendLine(Mathf.CeilToInt(_defaultArray[index] * _addArray[index]).ToString());

            return sb.ToString();
        }
        string GetHealCountDetail()
        {
            StringBuilder sb = new StringBuilder();
            int index = (int)UpgradeType.HealCount;
            sb.Append("현재 힐 횟수 : ");
            sb.AppendLine(GetCurrentUpgradeValue(index).ToString());

            sb.Append("업그레이드 증가량 : ");
            sb.Append(Mathf.CeilToInt(_defaultArray[index] * _addArray[index]));
            sb.AppendLine(" 회");

            sb.Append("힐 잔량 보상 : ");
            sb.Append(_healFee);
            sb.Append(" Gold / 1회");

            return sb.ToString();
        }
    }
}
