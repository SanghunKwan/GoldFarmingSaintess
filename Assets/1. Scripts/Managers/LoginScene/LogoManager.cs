using UnityEngine;
using GFSUtilities.UI;

namespace GFSManagers
{

    public class LogoManager : BaseBGWindowManager<LogoWindow, LogoManager, BGManager>
    {
        public override void InitManager(BGManager bgManager)
        {
            base.InitManager(bgManager);

            MakeLogoWindow();
        }


        public void MakeLogoWindow()
        {
            GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Logo, _bgManager.transform);
            _window = go.GetComponent<LogoWindow>();
            _window.InitWindow(this);

        }

        public void CallWindow()
        {
            _bgManager.CallUI(0, _window);
        }

        public void EndWindow()
        {
            _window.FadeOut();
            _bgManager.ReleaseUI();
            LoginSceneManager.Instance.LoginReady();
        }
    }
}