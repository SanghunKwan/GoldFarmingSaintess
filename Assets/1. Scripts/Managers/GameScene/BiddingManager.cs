using GFSUtilities.Effect;
using GFSUtilities.Item;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using GFSUtilities.UI;
using System;
using System.Text;
using UnityEngine;

namespace GFSManagers
{
    public class BiddingManager : BaseBGWindowManager<BiddingWindow, BiddingManager, NoneBGManager>
    {
        int _currentCost;
        int _itemLength;
        int _agenda;

        int _secondPrize;
        float _moveTime;

        bool _isSent;

        public HostManager _hostManager { get; set; }
        public InventoryManager _inventoryManager { get; set; }
        public ExplainManager _explainManager { get; set; }

        public event Action<int> CurrentCostChangeEvent;



        public int _CurrentCost
        {
            get => _currentCost;
            set
            {
                if (value < 0) return;

                _currentCost = value;
                CurrentCostChangeEvent(_currentCost);
                SendEmotion(EmotionType.Bidding);
            }
        }
        public int _Agenda
        {
            get => _agenda;
            set
            {
                _agenda = value;
                _window.SetData(_inventoryManager._itemData[_agenda].cost, _inventoryManager.GetItemSprite(_agenda));
            }
        }


        public override void InitManager(NoneBGManager bgManager)
        {
            base.InitManager(bgManager);
            _itemLength = _inventoryManager._itemData.Length;
            TurnScriptableObject data = GameManager.Instance._TurnScriptableObject;
            _secondPrize = data._voteSecondPrize;
            _moveTime = data._prizeMoveTime;
        }

        public void CallWindow()
        {
            if (_window == null)
            {
                GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Bidding, _bgManager.transform);
                _window = go.GetComponent<BiddingWindow>();
                _window.InitWindow(this);
            }

            _currentCost = 0;
            CurrentCostChangeEvent(_currentCost);
            _bgManager.CallUI(1, _window);
            _hostManager.AlertServerNeedIndex(0, _itemLength);
            _isSent = false;
        }
        public void SendEmotion(EmotionType type)
        {
            _hostManager.SetEmotion(type);
        }
        public void SubmitCost()
        {
            SendEmotion(EmotionType.BidSubmit);
            _hostManager.SendRPC(new AnonymousVote { _value = _currentCost });
            _isSent = true;

            _window.AfterSent();
        }
        public void SetMarketPrice()
        {
            ref readonly Item itemData = ref _inventoryManager._itemData[_agenda];
            if (_currentCost == itemData.cost) return;

            _CurrentCost = itemData.cost;
        }
        public void CheckSent()
        {
            if (_isSent) return;

            SubmitCost();
        }
        public void CompareSecond(int secondCost, EffectManager effectManager, HPManager hpManager)
        {
            _window.FadeOut();

            if (_currentCost < secondCost)
            {
                _inventoryManager.FadeOut();
                return;
            }

            if (_currentCost == secondCost)
            {
                //2등상
                _bgManager._CurrentGold += _secondPrize;
                _inventoryManager.FadeOut();
            }
            //1등상
            else
            {
                //1등상
                _bgManager._CurrentGold -= _currentCost;
                if (_inventoryManager.HasEmptySlot(out int index))
                {
                    GameObject go = GameManager.Instance.InstantiateResourcePrefab(UIResourceType.FloatingImage, _bgManager.transform);
                    go.transform.position = _window._ItemImagePosition;

                    FloatingImage floating = go.GetComponent<FloatingImage>();
                    floating.InitImage(_inventoryManager.SlotPosition(index), _moveTime, _inventoryManager.GetItemSprite(_agenda));
                    floating.ArriveEvent += () =>
                    {
                        _inventoryManager.SetItem(index, _agenda);
                        _inventoryManager.FadeOut();
                    };

                    FollowCamEffect followEffect = go.GetComponent<FollowCamEffect>();
                    followEffect.InitEffect(effectManager._CamEffects[(int)CamEffectType.Leak], hpManager);
                }
            }
        }

        public void CallBiddingExplain()
        {
            _explainManager.ShowAtMousPosition(ExplainType.BiddingMoney, GetBiddingSystemExplain());
        }
        public void CallItemExplain()
        {
            _explainManager.ShowAtMousPosition(ExplainType.BiddingItem, _inventoryManager.GetItemExplain(_agenda));
        }

        string GetBiddingSystemExplain()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("차기 입찰 보상 : ");
            sb.Append(_secondPrize);
            sb.AppendLine(" Gold");


            return sb.ToString();
        }
    }
}