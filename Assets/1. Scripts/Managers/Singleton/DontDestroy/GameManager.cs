using GFSBattle;
using GFSUtilities.UI;
using GFSUtilities.Unit;
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
        [SerializeField] PrefabScriptableObject _uIResourcePrefabScriptableObject;
        [SerializeField] PrefabScriptableObject _unitPrefabScriptableObject;

        [SerializeField] SpriteScriptableObject _spriteScriptableObject;


        public UpgradeScriptableObjects _UpgradeObjects => _upgradeObjects;
        public EffectScriptableObject _EffectScriptableObject => _effectScriptableObject;
        public SettlementScriptableObject _SettlementScriptableObject => _settlementScriptableObject;
        public SelectDataScriptableObject _SelectDataScriptableObject => _selectDataScriptableObject;
        public SpriteScriptableObject _UISpriteScriptableObject => _spriteScriptableObject;

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
            _uIResourcePrefabScriptableObject = (PrefabScriptableObject)data._scriptableDatas[(int)GameManagerDataType.UIResourcePrefab];
            _unitPrefabScriptableObject = (PrefabScriptableObject)data._scriptableDatas[(int)GameManagerDataType.UnitPrefab];

            _spriteScriptableObject = (SpriteScriptableObject)data._scriptableDatas[(int)GameManagerDataType.UISprite];
        }

        public GameObject InstantiatePrefab(UIType type, Transform instantiateParent = null)
        {
            return Instantiate(_uIPrefabScriptableObject._prefabs[(int)type], instantiateParent);
        }
        public GameObject InstantiateResourcePrefab(UIResourceType type, Transform instantiateParent = null)
        {
            return Instantiate(_uIResourcePrefabScriptableObject._prefabs[(int)type], instantiateParent);
        }
        public GameObject InstantiateCharacterPrefab(UnitTypes type, Force force, Transform instantiateParent = null)
        {
            return Instantiate(_unitPrefabScriptableObject._prefabs[(int)type + ((force == Force.Enemy) ? 3 : -1)], instantiateParent);
        }
    }
}
