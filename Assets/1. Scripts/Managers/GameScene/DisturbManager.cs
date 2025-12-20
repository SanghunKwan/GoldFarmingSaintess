using GFSBattle;
using GFSUtilities.Protocol;
using GFSUtilities.UI;
using GFSUtilities.Unit;
using System;
using System.Collections;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;


namespace GFSManagers
{
    public class DisturbManager : BaseBGWindowManager<DisturbWindow, DisturbManager, BGManager>
    {
        int _disturbAllCount;

        int[] _disturbCount;
        int[] _disturbCost;
        int[] _disturbIntensity;


        int _downedEnemyCount;
        int[] _downedEnemyArray;


        int _disturbComprehensiveFee;
        int _disturbPlayerFee;

        DisturbCountFollower _disturbCountFollower;

        IEnumerator _IenumCallWindow;

        bool _isActive;


        public HostManager _hostManager { get; set; }
        public NoneBGManager _noneBGManager { get; set; }
        public ExplainManager _explainManager { get; set; }


        public override void InitManager(BGManager bgManager)
        {
            base.InitManager(bgManager);

            int length = (int)DisturbType.Max;
            _disturbCount = new int[length];
            _disturbCost = new int[length];
            _disturbIntensity = new int[length];

            var data = GameManager.Instance._SettlementScriptableObject;

            _disturbComprehensiveFee = data._disturbComprehensiveFee;
            _disturbPlayerFee = data._disturbPlayerFee;
            Array.Copy(data._disturbIntensity, _disturbIntensity, length);
        }

        public void CallWindow(SettlementManager manager)
        {
            if (_window == null)
            {
                GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Disturb, _bgManager.transform);
                _window = go.GetComponent<DisturbWindow>();
                _window.InitWindow(this);

                _disturbCountFollower = _window.GetComponent<DisturbCountFollower>();
                _disturbCountFollower.InitFollower(GetUpdatedCount);
            }
            _IenumCallWindow = _bgManager.CallUI(1, _window);
            _isActive = true;
            _hostManager.SendWaitPhaseEnd(GFSUtilities.ResourcesData.GamePhaseType.Battle);

            _disturbAllCount = 0;
            Array.Copy(_disturbCountFollower._lastDisturbCount, _disturbCount, _disturbCount.Length);

            _downedEnemyCount = manager.GetDeadEnemy(out _downedEnemyArray);
            SetRandomDisturbUnit();

            for (DisturbType type = DisturbType.MonsterSpawn; type < DisturbType.Max; type++)
            {
                UpdateCount(type);
                UpdateResult(type);
            }

        }
        void SetRandomDisturbUnit()
        {
            int starCountNum = (int)GetRandomDisturbUnit();

            _disturbIntensity[(int)DisturbType.MonsterSpawn] = starCountNum;
        }

        StarCount GetRandomDisturbUnit()
        {
            if (_downedEnemyCount == 0) return StarCount.Beginner;

            int num = Random.Range(0, _downedEnemyCount);

            for (int i = 0; i < _downedEnemyArray.Length; i++)
            {
                num -= _downedEnemyArray[i];
                if (num <= 0)
                    return (StarCount)(i + 1);
            }
            Debug.LogError("쓰러진 적 배열과 숫자 불일치");
            return StarCount.None;
        }
        void GetUpdatedCount(DisturbType type, int updatedCount)
        {
            _disturbCount[(int)type] = updatedCount;

            UpdateCount(type);
        }
        void UpdateCount(DisturbType type)
        {
            int index = (int)type;
            int newCalculated = _disturbCount[index] * _disturbComprehensiveFee + Mathf.RoundToInt(Mathf.Pow(_disturbAllCount, 2)) * _disturbPlayerFee;
            _disturbCost[index] = newCalculated;
            _window.UpdatePrice(type, newCalculated);
        }
        void UpdateResult(DisturbType type)
        {
            int index = (int)type;
            _window.UpdateResult(type, _disturbIntensity[index]);
        }
        public bool SelectDisturb(DisturbType type)
        {
            if (CantDisturb((int)type))
            {
                //비용 부족
                _noneBGManager.ErrorMoney();
                return false;
            }

            GameSceneManager.Instance.SetEmotion(GFSUtilities.ResourcesData.EmotionType.Annoying);
            _hostManager.SendRPC(new ClientDisturbRPC { _type = type, _intensity = _disturbIntensity[(int)type] });
            _disturbAllCount++;

            IncreaseExceptSelected(type);
            if (type == DisturbType.MonsterSpawn)
            {
                SetRandomDisturbUnit();
                UpdateResult(type);
            }

            return true;
        }
        bool CantDisturb(int index) => _disturbCost[index] > _noneBGManager._CurrentGold;

        void IncreaseExceptSelected(DisturbType type)
        {
            for (DisturbType i = DisturbType.MonsterSpawn; i < DisturbType.Max; i++)
            {
                if (i == type) continue;

                UpdateCount(i);
            }
        }

        public void FadeOut()
        {
            if (!_isActive) return;

            _isActive = false;
            _bgManager.ReleaseUI();

            if (_window.gameObject.activeSelf)
                _window.FadeOut();
            else
                _bgManager.StopCoroutine(_IenumCallWindow);
        }
        public void CallExplain(DisturbType type)
        {
            _explainManager.ShowAtMousPosition(ExplainType.DisturbSpawn + (int)type, GetDisturbSpawnDetail(type));
        }
        string GetDisturbSpawnDetail(DisturbType type)
        {
            StringBuilder sb = new StringBuilder();
            int index = (int)type;

            sb.Append("이번 턴 나의 방해 횟수 : ");
            sb.Append(_disturbAllCount);
            sb.AppendLine("회");

            sb.Append("현재 턴 방해 횟수 : ");
            sb.Append(_disturbCount[index]);
            sb.AppendLine("회");

            return sb.ToString();
        }
    }
}