using GFSBattle;
using GFSUtilities;
using GFSUtilities.Unit;
using GFSUtilities.Upgrade;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace GFSManagers
{
    public class GameSceneManager : MonoBehaviour
    {
        public static GameSceneManager Instance { get; private set; }

        //null로 체크할 경우 또 생성하고 삭제해야 하므로 Scene에서 반복하므로 구조 수정 필요.
        BattleManager _battleManager;
        EffectManager _effectManager;
        PlaceManager _placeManager;
        SettlementManager _settlementManager;
        SelectManager _selectManager;
        SpawnManager _spawnManager;
        TrainingManager _trainingManager;
        TurnManager _turnManager;

        [Header("씬 내 매니저")]
        [SerializeField] BGManager _bgManager;
        [SerializeField] NoneBGManager _noneBGManager;
        [SerializeField] HostManager _hostManager;

        [Header("씬 내 데이터")]
        [SerializeField] Transform _unitFolder;



        LinkedList<BaseUnit> _ally;
        LinkedList<BaseUnit> _enemy;

        float GetsqrDistance(in Vector3 vec1, in Vector3 vec2)
            => (vec1 - vec2).sqrMagnitude;

        private void Awake()
        {
            Instance = this;

            _ally = new LinkedList<BaseUnit>();
            _enemy = new LinkedList<BaseUnit>();

        }

        public void ReadyToStart()
        {
            _turnManager = new TurnManager();
            _turnManager.InitManager(_bgManager);

            _bgManager.InitManager();
            _noneBGManager.InitManager();
            _turnManager.CallTurnUI();

            //호스트 하나에 나머지는 다 클라이언트임. 이미 정해져있음.

            _hostManager.CreatePlayersUI(_bgManager.transform);
        }

        void SelectInit()
        {
            _selectManager = new SelectManager();
            _selectManager.InitManager(_bgManager);
            _selectManager.MakeBattle();
        }
        void PrepareNextStage()
        {
            _trainingManager = new TrainingManager();
            _trainingManager.InitManager(_noneBGManager);
        }
        void StageInitReady()
        {
            _effectManager = new EffectManager();
            _effectManager.InitManager();
            _spawnManager = new SpawnManager();
            _spawnManager.InitManager(_unitFolder);
            _placeManager = new PlaceManager();
            _spawnManager.SpawnUnit(_selectManager._AllyUnits, Force.Ally);
            _spawnManager.SpawnUnit(_selectManager._EnemyUnits, Force.Enemy);
        }
        void InitBattle()
        {
            _battleManager = new BattleManager();
            _settlementManager = new SettlementManager();
            _settlementManager.InitManager(_bgManager);
            _settlementManager.SetData(_selectManager._Battle);
            _placeManager.EndPlacePhase();
            _placeManager = null;

            _battleManager.InitManger(_trainingManager._CurrentValue(UpgradeType.HealCount),
                                      _trainingManager._CurrentValue(UpgradeType.HealAmount));
        }

        public LinkedListNode<BaseUnit> EnrollUnit(BaseUnit unit)
        {
            return (unit._force == Force.Ally) ? _ally.AddLast(unit) : _enemy.AddLast(unit);
        }
        public void UnenrollUnit(LinkedListNode<BaseUnit> node)
        {
            LinkedList<BaseUnit> tempList = node.List;
            tempList.Remove(node);
            _settlementManager.AddInSettle(node);

            if (tempList.Count <= 0)
                BattleEndCall((tempList != _ally) ? _ally : _enemy);
        }

        public void BattleStart(float second)
        {
            InitBattle();

            foreach (var item in _ally)
                item.BattleStart(second);
            foreach (var item in _enemy)
                item.BattleStart(second);
        }



        public bool FindTarget(BaseUnit unit, out BaseUnit target)
        {
            LinkedList<BaseUnit> list = (unit._force == Force.Ally) ? _enemy : _ally;
            target = null;
            if (list.Count == 0) return false;

            target = FindCloseNode(list, unit);
            return true;
        }
        BaseUnit FindCloseNode(LinkedList<BaseUnit> list, BaseUnit unit)
        {
            float minDistance = float.MaxValue;
            BaseUnit target = null;
            foreach (var node in list)
            {
                float distance = GetsqrDistance(node.transform.position, unit.transform.position);
                if (distance >= minDistance) continue;

                minDistance = distance;
                target = node;
            }

            return target;
        }



        void BattleEndCall(LinkedList<BaseUnit> leftList)
        {
            StartCoroutine(GFSManager.WaitForSecond(2, () =>
            {
                foreach (var item in leftList)
                {
                    item.ClearInAlive();
                }
            }));

            StartCoroutine(GFSManager.WaitForSecond(3, StartSettlement));
        }

        #region BattleManager Transfer


        public void Attack(BaseUnit attacker, BaseUnit defender)
        {
            _battleManager.CalculateDamage(attacker, defender, (int)attacker._force);
        }
        public void ClickUnit(BaseUnit target)
        {
            if (_battleManager == null) return;

            _battleManager.HealUnit(target);
        }
        #endregion BattleManager Transfer

        #region EffectManager Transfer
        public GameObject GetEffect(BaseUnit unit, UnitEffectType type)
        {
            GameObject newEffect;

            switch (type)
            {
                case UnitEffectType.BaseEffect:
                    newEffect = GetBaseEffect(unit._force, unit._starCount);
                    break;

                case UnitEffectType.HealEffect:
                    newEffect = GetHealEffect(unit._force);
                    break;

                case UnitEffectType.WeaponEffect:
                    newEffect = GetHealEffect(unit._force);
                    break;

                default:
                    newEffect = null;
                    Debug.Log("알 수 없는 이펙트 요청");
                    break;
            }
            return newEffect;
        }
        GameObject GetBaseEffect(Force force, StarCount starCount)
        => _effectManager._BaseEffects[force][starCount];
        GameObject GetHealEffect(Force force)
         => _effectManager._HealEffects[(int)force];

        #endregion EffectManager Transfer

        #region PlaceManager Transfer
        public void DragInUnit(BaseUnit target)
        {
            if (_placeManager == null) return;

            _placeManager.DragInUnit(target);
        }
        public void DragOutUnit(BaseUnit target)
        {
            if (_placeManager == null) return;

            _placeManager.DragOutUnit(target);
        }
        #endregion PlaceManager Transfer

        #region SettlementManager Transfer
        void StartSettlement()
        {
            _settlementManager.SetData(_battleManager.GetResult(_ally.Count != 0));
            _settlementManager.CalculateSettlement();
            _settlementManager.ShowWindow();
        }
        #endregion SettlementManager Transfer
        #region SelectManager Transfer
        public void EndSelect()
        {
            PrepareNextStage();
        }
        #endregion SelectManager Transfer
        #region TrainingManager Transfer
        public void EndTraining()
        {
            _trainingManager.TrainingTimeOut();
            StageInitReady();
        }
        #endregion TraningManager Transfer
        #region TurnManager Transfer
        public void EndTurn()
        {
            SelectInit();
        }
        #endregion TurnManager Transfer

        public static Unity.Entities.Entity _entity;
        public void ClickButton()
        {
            using var commandBuffer = new EntityCommandBuffer(Allocator.Temp);
            commandBuffer.SetComponentEnabled<PlayerProtocol>(_entity, true);
            commandBuffer.Playback(ClientServerBootstrap.ClientWorld.EntityManager);
        }
    }
}

