using GFSBattle;
using GFSUtilities.UI;
using GFSUtilities.Unit;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace GFSManagers
{
    public class GameManager : BaseDontDestoryManager<GameManager>
    {

        [Header("게임 내 데이터")]
        [SerializeField] EffectScriptableObject _effectScriptableObject;
        [SerializeField] SettlementScriptableObject _settlementScriptableObject;
        [SerializeField] SelectDataScriptableObject _selectDataScriptableObject;
        [SerializeField] TrainingManagerDataScriptableObject _trainingDataScriptableObject;
        [SerializeField] TurnScriptableObject _turnScriptableObject;
        [SerializeField] ServerDataScriptableObject _serverScriptableObject;

        [SerializeField] PrefabScriptableObject _uIPrefabScriptableObject;
        [SerializeField] PrefabScriptableObject _uIResourcePrefabScriptableObject;
        [SerializeField] PrefabScriptableObject _unitPrefabScriptableObject;

        [SerializeField] SpriteScriptableObject _spriteScriptableObject;

        public EffectScriptableObject _EffectScriptableObject => _effectScriptableObject;
        public SettlementScriptableObject _SettlementScriptableObject => _settlementScriptableObject;
        public SelectDataScriptableObject _SelectDataScriptableObject => _selectDataScriptableObject;
        public TrainingManagerDataScriptableObject _TrainingDataScriptableObject => _trainingDataScriptableObject;
        public TurnScriptableObject _TurnScriptableObject => _turnScriptableObject;
        public ServerDataScriptableObject _ServerScriptableObject => _serverScriptableObject;

        public SpriteScriptableObject _UISpriteScriptableObject => _spriteScriptableObject;



        public override void InitManager()
        {
            GameManagerDataScriptableObject data = Resources.Load<GameManagerDataScriptableObject>("GameManagerDataScriptableObject");

            _effectScriptableObject = (EffectScriptableObject)data._scriptableDatas[(int)GameManagerDataType.Effect];
            _settlementScriptableObject = (SettlementScriptableObject)data._scriptableDatas[(int)GameManagerDataType.Settle];
            _selectDataScriptableObject = (SelectDataScriptableObject)data._scriptableDatas[(int)GameManagerDataType.SelectData];
            _trainingDataScriptableObject = (TrainingManagerDataScriptableObject)data._scriptableDatas[(int)GameManagerDataType.TrainingData];
            _turnScriptableObject = (TurnScriptableObject)data._scriptableDatas[(int)GameManagerDataType.Turn];
            _serverScriptableObject = (ServerDataScriptableObject)data._scriptableDatas[(int)GameManagerDataType.Server];

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
