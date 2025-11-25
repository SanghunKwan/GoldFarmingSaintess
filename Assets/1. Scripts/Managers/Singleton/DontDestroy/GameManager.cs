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
        EffectScriptableObject _effectScriptableObject;
        SettlementScriptableObject _settlementScriptableObject;
        SelectDataScriptableObject _selectDataScriptableObject;
        TrainingManagerDataScriptableObject _trainingDataScriptableObject;
        TurnScriptableObject _turnScriptableObject;

        ServerDataScriptableObject _serverScriptableObject;

        PrefabScriptableObject _uIPrefabScriptableObject;
        PrefabScriptableObject _uIResourcePrefabScriptableObject;
        PrefabScriptableObject _unitPrefabScriptableObject;

        SpriteScriptableObject _spriteScriptableObject;
        TextScriptableObject _textScriptableObject;
        ColorScriptableObject _playerColorScriptableObject;

        SceneChangeDataScriptableObject _sceneChangeDataScriptableObject;

        public EffectScriptableObject _EffectScriptableObject => _effectScriptableObject;
        public SettlementScriptableObject _SettlementScriptableObject => _settlementScriptableObject;
        public SelectDataScriptableObject _SelectDataScriptableObject => _selectDataScriptableObject;
        public TrainingManagerDataScriptableObject _TrainingDataScriptableObject => _trainingDataScriptableObject;
        public TurnScriptableObject _TurnScriptableObject => _turnScriptableObject;

        public ServerDataScriptableObject _ServerScriptableObject => _serverScriptableObject;

        public SpriteScriptableObject _UISpriteScriptableObject => _spriteScriptableObject;
        public TextScriptableObject _UITextScriptableObject => _textScriptableObject;
        public ColorScriptableObject _PlayerColorScriptableObject => _playerColorScriptableObject;

        public SceneChangeDataScriptableObject _SceneChangeDataScriptableObject => _sceneChangeDataScriptableObject;

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
            _textScriptableObject = (TextScriptableObject)data._scriptableDatas[(int)GameManagerDataType.UIText];

            _playerColorScriptableObject = (ColorScriptableObject)data._scriptableDatas[(int)GameManagerDataType.PlayerColor];

            _sceneChangeDataScriptableObject = (SceneChangeDataScriptableObject)data._scriptableDatas[(int)GameManagerDataType.SceneChangeData];
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
