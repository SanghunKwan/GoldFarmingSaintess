using GFSUtilities.Protocol;
using GFSUtilities.UI;
using UnityEngine;


namespace GFSManagers
{
    public class TurnManager : BaseBGWindowManager<TurnWindow, TurnManager, BGManager>
    {
        public int _currentTurn { get; private set; }
        public int _maxTurn { get; set; }


        public override void InitManager(BGManager bgManager)
        {
            base.InitManager(bgManager);

            _currentTurn = 0;
        }

        public void CallTurnUI()
        {
            if (_window == null)
            {
                GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Turn, _bgManager.transform);
                _window = go.GetComponent<TurnWindow>();
                _window.InitWindow(this);
                _window.SetMaxTurn(_maxTurn);
            }

            _window.SetCurrentTurn(++_currentTurn);

            if (_currentTurn > _maxTurn)
            {
                GameSceneManager.Instance.GameComplete();
                return;
            }

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