using GFSUtilities.Item;
using GFSUtilities.Upgrade;
using UnityEngine;


namespace GFSManagers
{
    public class TrainingManager : BaseBGWindowManager<TrainingWindow, TrainingManager>
    {



        int[] _defaultArray;
        float[] _addArray;

        int[] _currentUpgrades;

        public Item[] _items { get; private set; }
        public int _HealCount
        {
            get => 1;
        }
        public int _HealAmount
        {
            get => 1;
        }



        public override void InitManager(BGManager _bgManager)
        {
            base.InitManager(_bgManager);

            TrainingManagerDataScriptableObject data = GameManager.Instance._TrainingDataScriptableObject;

            _defaultArray = data.defaultArray;
            _addArray = data.addArray;

            _currentUpgrades = new int[(int)UpgradeType.Max];
            _items = new Item[data._inventorySize];
        }

        public void ReadyToTraining()
        {

        }

    }
}