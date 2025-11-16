using GFSBattle;
using GFSUtilities.UI;
using GFSUtilities.Upgrade;
using UnityEngine;

namespace GFSManagers
{
    public class GameManager : BaseDontDestoryManager<GameManager>

    {
        [Header("게임 내 데이터")]
        [SerializeField] UpgradeScriptableObjects _upgradeObjects;
        [SerializeField] EffectScriptableObject _effectScriptableObject;
        [SerializeField] SettlementScriptableObject _settlementScriptableObject;
        [SerializeField] SelectDataScriptableObject _selectDataScriptableObject;

        [SerializeField] PrefabScriptableObject _uIPrefabScriptableObject;


        public UpgradeScriptableObjects _UpgradeObjects => _upgradeObjects;
        public EffectScriptableObject _EffectScriptableObject => _effectScriptableObject;
        public SettlementScriptableObject _SettlementScriptableObject => _settlementScriptableObject;
        public SelectDataScriptableObject _SelectDataScriptableObject => _selectDataScriptableObject;

        public int[] _healUpgrade { get; private set; }


        public override void InitManager()
        {
            _healUpgrade = new int[(int)UpgradeType.Max];

            GameManagerDataScriptableObject data = Resources.Load<GameManagerDataScriptableObject>("GameManagerDataScriptableObject");

            _upgradeObjects = (UpgradeScriptableObjects)data._scriptableDatas[(int)GameManagerDataType.Upgrade];
            _effectScriptableObject = (EffectScriptableObject)data._scriptableDatas[(int)GameManagerDataType.Effect];
            _settlementScriptableObject = (SettlementScriptableObject)data._scriptableDatas[(int)GameManagerDataType.Settle];
            _selectDataScriptableObject = (SelectDataScriptableObject)data._scriptableDatas[(int)GameManagerDataType.SelectData];

            _uIPrefabScriptableObject = (PrefabScriptableObject)data._scriptableDatas[(int)GameManagerDataType.UIPrefab];
        }

        public GameObject InstantiatePrefab(UIType type, Transform instantiateParent = null)
        {
            return Instantiate(_uIPrefabScriptableObject._prefabs[(int)type], instantiateParent);
        }
    }
}
