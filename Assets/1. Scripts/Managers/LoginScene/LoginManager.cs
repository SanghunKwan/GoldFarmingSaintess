using UnityEngine;
using GFSUtilities.UI;

namespace GFSManagers
{
    public class LoginManager : BaseBGWindowManager<LoginWindow, LoginManager, BGManager>
    {
        public override void InitManager(BGManager bgManager)
        {
            base.InitManager(bgManager);

        }

        public void MakeWindow()
        {
            GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Login, _bgManager.transform);
            _window = go.GetComponent<LoginWindow>();
            _window.InitWindow(this);
        }
    }
}

