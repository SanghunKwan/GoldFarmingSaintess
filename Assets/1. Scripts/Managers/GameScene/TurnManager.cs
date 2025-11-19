using UnityEngine;
using GFSUtilities.UI;


namespace GFSManagers
{
    public class TurnManager : BaseBGWindowManager<TurnWindow, TurnManager, BGManager>
    {
        public int _currentTurn { get; private set; }
        int _maxTurn;


        public override void InitManager(BGManager bgManager)
        {
            base.InitManager(bgManager);

            TurnScriptableObject data = GameManager.Instance._TurnScriptableObject;

            _maxTurn = data._maxTurn;

            _currentTurn = 1;
        }

        public void CallTurnUI()
        {
            GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Turn, _bgManager.transform);
            _window = go.GetComponent<TurnWindow>();
            _window.InitWindow(this);
            _window.SetMaxTurn(_maxTurn);

            _bgManager.CallUI(1.5f, _window);
        }

        public void FadeOut()
        {
            _window.FadeOut();
            _bgManager.ReleaseUI();
            GameSceneManager.Instance.EndTurn();
        }

    }
}