using GFSBattle;
using GFSUtilities;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using GFSUtilities.Unit;
using GFSUtilities.Upgrade;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        InventoryManager _inventoryManager;
        TrainingManager _trainingManager;
        TurnManager _turnManager;
        DisturbManager _disturbManager;
        BiddingManager _biddingManager;

        [Header("씬 내 매니저")]
        [SerializeField] BGManager _bgManager;
        [SerializeField] NoneBGManager _noneBGManager;
        [SerializeField] HostManager _hostManager;
        [SerializeField] PlaneManager _planeManager;
        [SerializeField] HPManager _hpManager;
        [SerializeField] ExplainManager _explainManager;


        [Header("씬 내 데이터")]
        [SerializeField] Transform _unitFolder;
        [SerializeField] GraphicColorController _quitButton;



        LinkedList<BaseUnit> _ally;
        LinkedList<BaseUnit> _enemy;


        GamePhaseType _currentPhase;

        private void Awake()
        {
            Instance = this;

            _ally = new LinkedList<BaseUnit>();
            _enemy = new LinkedList<BaseUnit>();

        }
        private void Start()
        {
            _hostManager.InitManager();
        }

        public void ReadyToStart(in AllClientReady readyData)
        {
            _turnManager = new TurnManager();
            _turnManager.InitManager(_bgManager);
            _turnManager._maxTurn = readyData.maxRound;

            _bgManager.InitManager();
            _noneBGManager.InitManager(readyData.defaultGold);
            SetPhase(GamePhaseType.Turn);

            //호스트 하나에 나머지는 다 클라이언트임. 이미 정해져있음.

            _hostManager.CreatePlayersUI(_bgManager.transform, readyData.playerCount);
        }
        public void SetPhase(GamePhaseType type)
        {
            _currentPhase = type;
            _explainManager.HideWindow();

            switch (type)
            {
                case GamePhaseType.Turn:
                    _turnManager.CallTurnUI();
                    break;
                case GamePhaseType.Event:
                    //이벤트 추가 예정.
                    Debug.Log("이벤트 오픈");
                    break;
                case GamePhaseType.Select:
                    SelectInit();
                    Debug.Log("선택창 오픈");
                    break;
                case GamePhaseType.Place:
                    SelectEnd();
                    break;
                case GamePhaseType.Battle:
                    BattleStart(1);
                    break;
                case GamePhaseType.Settlement:
                    StartSettlement();
                    break;
                case GamePhaseType.Bidding:
                    StartBidding();
                    break;
                case GamePhaseType.BidCalculate:
                    EndBidding();
                    break;
            }
        }
        void SelectInit()
        {
            if (_selectManager == null)
            {
                _selectManager = new SelectManager();
                _selectManager.InitManager(_bgManager);
                _selectManager.MakeBattle();
                _selectManager._explainManager = _explainManager;

                _hostManager.CreateTimerUI(_bgManager.transform);

                _explainManager.InitManager();
                _explainManager._hpManager = _hpManager;

                _hpManager.InitManager();
            }
            else
                _selectManager.MakeBattle();

            _hostManager.FadeInTimerUI();
        }
        void SelectEnd()
        {
            _hostManager.ShakeTimerUI();
            //아직 전투를 선택하지 않았을 때
            if (_selectManager._IsSelecting)
            {
                _selectManager.ChooseCurrentData();
                StageInitReady();

                if (_trainingManager == null)
                {
                    _trainingManager = new TrainingManager();
                    _trainingManager.InitManager(_noneBGManager);
                    _trainingManager._explainManager = _explainManager;
                }
            }
            else
                EndTraining();
        }
        void PrepareNextStage()
        {
            if (_trainingManager == null)
            {
                _trainingManager = new TrainingManager();
                _trainingManager.InitManager(_noneBGManager);
                _trainingManager._explainManager = _explainManager;
            }
            _trainingManager.ShowWindow();

            if (_inventoryManager == null)
            {
                _inventoryManager = new InventoryManager();
                _inventoryManager.InitManager(_noneBGManager);
                _inventoryManager._explainManager = _explainManager;
                _inventoryManager._selectManager = _selectManager;
            }
            _inventoryManager.CallUI(0);
        }
        void StageInitReady()
        {
            if (_effectManager == null)
            {
                _effectManager = new EffectManager();
                _effectManager.InitManager();
                _hostManager._effectManager = _effectManager;

                _planeManager.InitManager(_effectManager);

                _spawnManager = new SpawnManager();
                _spawnManager.InitManager(_unitFolder, _planeManager);

                _placeManager = new PlaceManager();
                _placeManager.InitManager(_planeManager);

            }

            _placeManager.ActivateManager();
            _planeManager.SetActiveSlots(true);
            _planeManager.SetSlotsState(PlaneManager.SlotStateType.UpDown);

            _spawnManager.SpawnUnit(_selectManager._AllyUnits, Force.Ally);
            _spawnManager.SpawnUnit(_selectManager._EnemyUnits, Force.Enemy);
            _spawnManager.ShuffleEnemy(_enemy);
        }
        void InitBattle()
        {
            if (_settlementManager == null)
            {
                _settlementManager = new SettlementManager();
                _settlementManager.InitManager(_bgManager);
                _settlementManager._noneBGManager = _noneBGManager;
                _settlementManager._hostManager = _hostManager;
                _settlementManager._selectManager = _selectManager;

                _hostManager._hpManager = _hpManager;
            }
            _settlementManager.SetData(_selectManager._Battle);

            _placeManager.EndPlacePhase();
            _planeManager.SetActiveSlots(false);

            _hpManager.MakeHPBar(_ally, Force.Ally);
            _hpManager.MakeHPBar(_enemy, Force.Enemy);

            _battleManager = new BattleManager();
            _battleManager.InitManager();
            _battleManager.UpdateManager(_trainingManager._CurrentValue(UpgradeType.HealCount),
                                        _trainingManager._CurrentValue(UpgradeType.HealAmount));

            if (_inventoryManager != null)
            {
                _inventoryManager.BindBattleManager(_battleManager);
                _selectManager.ClearAdditionalWeight();
            }
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

            _hostManager.AlertServerNeedGhostPrefab(PrefabGhostType.DisturbCounter);
            _hostManager.ShakeTimerUI();
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
        float GetsqrDistance(in Vector3 vec1, in Vector3 vec2)
            => (vec1 - vec2).sqrMagnitude;


        void BattleEndCall(LinkedList<BaseUnit> leftList)
        {
            _hpManager.BattleEnd();
            _battleManager._isBattleEnd = true;
            StartCoroutine(GFSManager.WaitForSecond(0.5f, () =>
            {
                foreach (var item in leftList)
                {
                    item.ClearInAlive(false);
                }
            }));
            CallDisturbWindow();

        }
        void CallDisturbWindow()
        {
            if (_disturbManager == null)
            {
                _disturbManager = new DisturbManager();
                _disturbManager.InitManager(_bgManager);
                _disturbManager._noneBGManager = _noneBGManager;
                _disturbManager._hostManager = _hostManager;
                _disturbManager._explainManager = _explainManager;
            }
            _disturbManager.CallWindow(_settlementManager);
        }

        #region BattleManager Transfer


        public void Attack(BaseUnit attacker, BaseUnit defender)
        {
            _battleManager.CalculateDamage(attacker, defender, (int)attacker._force - 1);
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
                    newEffect = GetWeaponEffect(unit._type);
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
        GameObject GetBuffEffect(Force force)
         => _effectManager._BuffEffects[(int)force];
        GameObject GetWeaponEffect(UnitTypes type)
            => _effectManager._WeaponEffects[Force.None][type];

        #endregion EffectManager Transfer

        #region PlaceManager Transfer
        public void DragInUnit(BaseUnit target)
        {
            if (!_placeManager._enabled) return;

            _placeManager.DragInUnit(target);
        }
        public void DragOutUnit(BaseUnit target)
        {
            if (!_placeManager._enabled) return;

            _placeManager.DragOutUnit(target);
        }
        #endregion PlaceManager Transfer

        #region SettlementManager Transfer
        void StartSettlement()
        {
            _settlementManager.SetData(_battleManager.GetResult(_enemy.Count == 0, _ally.Count == 0));
            _settlementManager.CalculateSettlement();
            _settlementManager.ShowWindow(_hostManager.FadeInTimerUI);

            _hostManager.FadeOutTimerUI();

            if (_battleManager._isBattleEnd)
                _disturbManager.FadeOut();
            else
            {
                _hpManager.BattleEnd();

                foreach (var item in _ally)
                    item.ClearInAlive(true);
                foreach (var item in _enemy)
                    item.ClearInAlive(false);
            }

            _battleManager = null;
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
            _inventoryManager.FadeOut();
            StageInitReady();
        }
        #endregion TraningManager Transfer
        #region TurnManager Transfer
        public void EndTurn()
        {
            _hostManager.AlertServerPhaseEnd(GamePhaseType.Turn);
            _hostManager.AlertServerNeedGhostPrefab(PrefabGhostType.Timer);
        }
        #endregion TurnManager Transfer
        #region HostManager Transfer
        public void SetTimer(float time)
        {
            _hostManager.SetTimer(time);
        }
        public void AlarmTimer()
        {
            _hostManager.AlertServerPhaseEnd(_currentPhase);
        }
        public void TransferPlayerProtocols(in PlayerProtocol protocol, int playerIndex)
        {
            if (_hostManager == null) return;

            _hostManager.SetMoney(protocol._gold, playerIndex);
            _hostManager.ShowEmotion(protocol._emotionType, playerIndex);

        }
        public void TransferDisturb(in ClientDisturbRPC disturb)
        {
            if (_battleManager._isBattleEnd) return;

            //_hostManager.ShowEmotion(protocol._emotionType, playerIndex);
            //전투 매니저 상호작용.
            SetEmotion(EmotionType.Anger);

            switch (disturb._type)
            {
                case DisturbType.MonsterSpawn:

                    _spawnManager.DisturbSpawnUnit((StarCount)disturb._intensity, _effectManager);
                    break;
                case DisturbType.MonsterBuff:
                    _battleManager.ReinforceDamage(_enemy, disturb._intensity, Instantiate(GetBuffEffect(Force.Enemy)));
                    break;
                case DisturbType.HeroHurt:
                    _battleManager.DamagePercent(_ally, disturb._intensity, Instantiate(GetBuffEffect(Force.Ally)));
                    break;
            }
        }

        public void SetEmotion(EmotionType type)
        {
            _hostManager.SetEmotion(type);
        }
        public void ShowBidResult(in FixedList32Bytes<AnonymousVoteBuffer> buffer)
        {
            _hostManager.AlertServerPhaseEnd(GamePhaseType.Bidding);
            _hostManager.ShowCostVote(buffer);
        }

        public void GameComplete()
        {
            _hostManager.AlertGameComplete();
        }
        public void ShowGameResult(in FixedList32Bytes<RaceResultBuffer> buffer)
        {
            _hostManager.ShowGameResult(buffer);

            StartCoroutine(GFSManager.WaitForSecond(2, () =>
            {
                _hostManager.ClearWorld();
                _quitButton.gameObject.SetActive(true);
                _quitButton.FadeGraphicAtOnce(0, 0, 0);
                _quitButton.FadeGraphicAtOnce(0, 1, 0.5f);
            }));
        }
        #endregion HostManager Transfer
        #region BiddingManager Transfer
        public void StartBidding()
        {
            if (_inventoryManager == null)
            {
                _inventoryManager = new InventoryManager();
                _inventoryManager.InitManager(_noneBGManager);
                _inventoryManager._explainManager = _explainManager;
                _inventoryManager._selectManager = _selectManager;
            }
            _inventoryManager.CallUI(1);

            if (_biddingManager == null)
            {
                _biddingManager = new BiddingManager();
                _biddingManager._inventoryManager = _inventoryManager;
                _biddingManager.InitManager(_noneBGManager);
                _biddingManager._hostManager = _hostManager;
                _biddingManager._explainManager = _explainManager;
            }
            _biddingManager.CallWindow();

            _hostManager.ShakeTimerUI();
            _settlementManager.HideWindow();
            _settlementManager.DisposeFormerTurn();
        }
        public void SetAgenda(int index)
        {
            _biddingManager._Agenda = index;
        }

        public void EndBidding()
        {
            _biddingManager.CheckSent();
        }

        public void BiddingCast(int secondCost)
        {
            _biddingManager.CompareSecond(secondCost, _effectManager, _hpManager);

            DisposeFormerList(_ally);
            DisposeFormerList(_enemy);

            StartCoroutine(GFSManager.WaitForSecond(2, () => _hostManager.AlertServerPhaseEnd(GamePhaseType.BidCalculate)));
        }
        #endregion BiddingManager Transfer
        public int GetMoney => _noneBGManager._CurrentGold;
        public void DisposeFormerList(LinkedList<BaseUnit> list)
        {
            while (list.Count > 0)
            {
                var node = list.First;

                list.RemoveFirst();
                Destroy(node.Value.gameObject);
            }
        }
        public void ClickQuitButton()
        {
            SceneManager.LoadScene(0);
        }

    }
}

