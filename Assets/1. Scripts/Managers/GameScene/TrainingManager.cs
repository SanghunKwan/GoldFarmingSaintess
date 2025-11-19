using GFSUtilities.Item;
using GFSUtilities.Upgrade;
using UnityEngine;
using GFSUtilities.UI;


namespace GFSManagers
{
    public class TrainingManager : BaseBGWindowManager<TrainingWindow, TrainingManager, NoneBGManager>
    {
        int[] _defaultArray;
        int[] _defaultPriceArray;
        float[] _addArray;

        float _exponent;


        public int[] _currentUpgrades { get; private set; }
        public Item[] _items { get; private set; }
        public int _CurrentValue(UpgradeType type) => GetCurrentUpgradeValue((int)type);
        public int _NextUpgradePrice(UpgradeType type) => GetPrice((int)type);


        int GetCurrentUpgradeValue(int index) => Mathf.CeilToInt(_defaultArray[index] * (1f + (_addArray[index] * _currentUpgrades[index])));
        int GetPrice(int index) => Mathf.CeilToInt(_defaultPriceArray[index] * Mathf.Pow(_exponent, _currentUpgrades[index]));

        public override void InitManager(NoneBGManager _bgManager)
        {
            base.InitManager(_bgManager);

            TrainingManagerDataScriptableObject data = GameManager.Instance._TrainingDataScriptableObject;

            _defaultArray = data.defaultArray;
            _addArray = data.addArray;
            _defaultPriceArray = data.defaultPrices;
            _exponent = data.exponent;

            _currentUpgrades = new int[(int)UpgradeType.Max];
            _items = new Item[data._inventorySize];

            ReadyToTraining();
        }
        public void ReadyToTraining()
        {
            GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Training, _bgManager.transform);
            _window = go.GetComponent<TrainingWindow>();
            _window.InitWindow(this);
            _window.FadeIn();
            //_bgManager.CallUI(0.5f, _window);
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
    }
}
