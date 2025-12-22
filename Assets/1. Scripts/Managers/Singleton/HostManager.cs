using GFSManagers;
using GFSUtilities;
using GFSUtilities.Effect;
using GFSUtilities.Protocol;
using GFSUtilities.ResourcesData;
using GFSUtilities.UI;
using System.Collections;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;




public class HostManager : MonoBehaviour
{
    GameManager _manager;

    PlayersUI _playersUI;
    TimerUI _timerUI;

    bool _isHost;
    float _voteOpenDelay;

    public string _nickname { get; private set; }
    EntityQuery[] _ghostQuerys;

    EntityQuery _inputQuery;

    EmotionType[] _playersEmotion;
    SpriteScriptableObject _spriteData;
    EntityManager _em;

    IEnumerator _ienum;
    Color[] _colors;

    public EffectManager _effectManager { get; set; }
    public HPManager _hpManager { get; set; }


    public void InitManager()
    {
        _manager = GameManager.Instance;
        var serverData = _manager._ServerScriptableObject;
        var data = _manager._SceneChangeDataScriptableObject;

        Debug.Log("Start");
        _em = ClientServerBootstrap.ClientWorld.EntityManager;
        _inputQuery = _em.CreateEntityQuery(typeof(GoldInputData), typeof(GhostOwnerIsLocal));

        if (data._nameIndex == 1)
        {
            _isHost = true;

            var em = ClientServerBootstrap.ServerWorld.EntityManager;

            var query = em.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            ref var driver = ref query.GetSingletonRW<NetworkStreamDriver>().ValueRW;

            _ghostQuerys = new EntityQuery[]
            {
                em.CreateEntityQuery(typeof(PlayerTimer)),
                em.CreateEntityQuery(typeof(DisturbCounter))
            };

            query.Dispose();
        }
        else
            _isHost = false;

        {
            data = _manager._SceneChangeDataScriptableObject;
            JoinRelay(data._joinCode);
        }

        _spriteData = _manager._UISpriteScriptableObject;
        _voteOpenDelay = _manager._TurnScriptableObject._voteOpenDelay;
        _colors = _manager._PlayerColorScriptableObject._color;
    }
    public async void JoinRelay(string joinCode)
    {
        var allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        var em = ClientServerBootstrap.ClientWorld.EntityManager;

        var relay = AllocationUtils.ToRelayServerData(allocation, "dtls");
        var settings = DefaultDriverBuilder.GetNetworkClientSettings();
        settings.WithRelayParameters(ref relay);
        var netDebug = em.CreateEntityQuery(typeof(NetDebug)).GetSingleton<NetDebug>();
        var driverStore = new NetworkDriverStore();
        DefaultDriverBuilder.RegisterClientUdpDriver(ClientServerBootstrap.ClientWorld, ref driverStore, netDebug, settings);
        var networkStreamDriver = em.CreateEntityQuery(typeof(NetworkStreamDriver)).GetSingleton<NetworkStreamDriver>();
        networkStreamDriver.ResetDriverStore(ClientServerBootstrap.ClientWorld.Unmanaged, ref driverStore);

        networkStreamDriver.Connect(em, relay.Endpoint);
    }
    #region HostAlert
    public void AlertServerPhaseEnd(GamePhaseType type)
    {
        if (!_isHost) return;

        var em = ClientServerBootstrap.ServerWorld.EntityManager;

        var entity = em.CreateEntity(typeof(HostPhaseEnd));
        em.AddComponentData(entity, new HostPhaseEnd { _phase = type });
    }
    public void AlertServerNeedGhostPrefab(PrefabGhostType type)
    {
        if (!_isHost) return;

        var em = ClientServerBootstrap.ServerWorld.EntityManager;

        if (!_ghostQuerys[(int)type].IsEmpty)
            return;

        var entity = em.CreateEntity(typeof(HostNeedGhostPrefab));
        em.AddComponentData(entity, new HostNeedGhostPrefab { _type = type });
    }
    public void AlertServerNeedIndex(int inclusiveBottom, int exclusiveTop)
    {
        if (!_isHost) return;

        var em = ClientServerBootstrap.ServerWorld.EntityManager;

        var entity = em.CreateEntity(typeof(ClientVoteIndex));
        em.AddComponentData(entity, new ClientVoteIndex { _inclusiveBottom = inclusiveBottom, _exclusiveTop = exclusiveTop });
    }
    public void AlertGameComplete()
    {
        if (!_isHost) return;

        var em = ClientServerBootstrap.ServerWorld.EntityManager;

        var entity = em.CreateEntity(typeof(GameComplete));
    }
    #endregion HostAlert
    public void CreatePlayersUI(Transform bgTr, int playerCount)
    {
        var data = _manager._SceneChangeDataScriptableObject;
        GameObject go = _manager.InstantiatePrefab(UIType.PlayersUI, bgTr);
        _playersUI = go.GetComponent<PlayersUI>();
        _playersUI.InitUI(data._names, data._nameIndex, out string[] names, _colors);
        _playersUI.DisableCountOver(playerCount);
        _playersEmotion = new EmotionType[playerCount];

        _nickname = names[data._nameIndex - 1];
    }
    public void SetMoney(int gold, int playerIndex)
    {
        if (_playersUI == null) return;
        _playersUI._slots[playerIndex - 1].ShowMoney(gold);
    }

    public void CreateTimerUI(Transform bgTr)
    {
        GameObject go = _manager.InstantiatePrefab(UIType.TimerUI, bgTr);
        _timerUI = go.GetComponent<TimerUI>();
        _timerUI.InitUI(_colors[(int)ColorType.Red]);
    }
    public void FadeInTimerUI()
    {
        _timerUI.gameObject.SetActive(true);
        _timerUI.FadeIn(() => SetTimerEnable(true));
        _timerUI.ResetTimer();
    }
    public void FadeOutTimerUI()
    {
        _timerUI.FadeOut();
        SetTimerEnable(false);
    }
    public void ShakeTimerUI()
    {
        _timerUI.Shake(() =>
        {
            SetTimerEnable(true);
            _timerUI.ResetTimer();
        });
    }
    public void SetTimerEnable(bool isOn) =>
        ClientServerBootstrap.ClientWorld.Unmanaged.GetExistingSystemState<ClientTimerSystem>().Enabled = isOn;
    public void SetTimer(float leftTime)
    {
        if (_timerUI == null) return;

        _timerUI.SetCount(leftTime);
    }



    public void ShowEmotion(EmotionType type, int playerIndex)
    {
        int index = playerIndex - 1;
        if (_playersEmotion[index] == type) return;

        _playersEmotion[index] = type;
        if (type != EmotionType.None)
            _playersUI._slots[index].ShowEmotion(_spriteData._sprites[(int)UISpriteType.EmotionAnnoying + (int)type - 1]);
    }
    public void SetEmotion(EmotionType type)
    {
        ref var value = ref _inputQuery.GetSingletonRW<GoldInputData>().ValueRW;

        if (value.emotionType == type) return;

        if (_ienum != null)
            StopCoroutine(_ienum);
        _ienum = GFSManager.WaitForSecond(0.5f, () =>
                    _inputQuery.GetSingletonRW<GoldInputData>().ValueRW.emotionType = EmotionType.None);

        value.emotionType = type;
        StartCoroutine(_ienum);
    }
    public void SendWaitPhaseEnd(GamePhaseType type)
    {
        _em.Broadcast(new WaitOtherEndPhase { _currentPhase = type });
    }
    public void SendRPC<T>(T protocol) where T : unmanaged, IComponentData
    {
        _em.Broadcast(protocol);
    }

    public void ShowCostVote(in FixedList32Bytes<AnonymousVoteBuffer> sortedBuffer)
    {
        StartCoroutine(TraversalBuffer(sortedBuffer, GetRanking(sortedBuffer)));
        FadeOutTimerUI();
    }
    int GetRanking(in FixedList32Bytes<AnonymousVoteBuffer> sortedBuffer)
    {
        AnonymousVoteBuffer tempBuffer = sortedBuffer[0];
        int lastNum = tempBuffer._value;
        int ranking = 1;

        for (int i = 1; i < sortedBuffer.Length; i++)
        {
            tempBuffer = sortedBuffer[i];

            if (lastNum == tempBuffer._value) continue;

            lastNum = tempBuffer._value;
            ++ranking;
        }
        return ranking;
    }
    IEnumerator TraversalBuffer(FixedList32Bytes<AnonymousVoteBuffer> sortedBuffer, int ranking)
    {
        var waitReturn = new WaitForSeconds(_voteOpenDelay);

        int lastVote = -1;
        int secondVote = lastVote;
        AnonymousVoteBuffer tempBuffer;
        PlayersUISlot slot;
        yield return new WaitForSeconds(1.5f);

        for (int i = 0; i < sortedBuffer.Length; ++i)
        {
            tempBuffer = sortedBuffer[i];
            if (lastVote != tempBuffer._value)
            {
                lastVote = tempBuffer._value;
                --ranking;
                yield return waitReturn;
                //lastVote°¡ 2µî Á¡¼ö.
                if (ranking == 1)
                    secondVote = lastVote;
            }
            slot = _playersUI._slots[tempBuffer._beforeIndex - 1];
            slot.ShowCostVote(lastVote);
            if (ranking < 2)
            {
                GameObject effect = _effectManager._CamEffects[(int)CamEffectType.Explode];
                var eff = Instantiate(effect, _hpManager.WorldFollowUI(slot._RankingPosition), Quaternion.identity);
                var main = eff.GetComponent<ParticleSystem>().main;
                main.startColor = _colors[ranking];
                //1µî »¡°­ 2µî ÆÄ¶û
                Destroy(eff, 1);
            }
        }
        GameSceneManager.Instance.BiddingCast(secondVote);
    }

    public void ShowGameResult(in FixedList32Bytes<RaceResultBuffer> buffer)
    {
        int currentRanking = 0;
        int currentGold = -1;
        RaceResultBuffer tempBuffer;
        for (int i = 0; i < buffer.Length; i++)
        {
            tempBuffer = buffer[i];
            if (currentGold != tempBuffer._gold)
            {
                currentRanking++;
                currentGold = tempBuffer._gold;
            }
            _playersUI._slots[tempBuffer._beforeIndex - 1].ShowResult(currentRanking);
        }
    }
    public void ClearWorld()
    {
        string name = ClientServerBootstrap.ClientWorld.Name;
        ClientServerBootstrap.ClientWorld.Dispose();
        ClientServerBootstrap.CreateClientWorld(name);

        if (_isHost)
            ClientServerBootstrap.ServerWorld.Dispose();
    }
}
