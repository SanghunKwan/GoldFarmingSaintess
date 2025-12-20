using UnityEngine;

public abstract class BaseBGWindowManager<TWindow, TManager, TBGManager> where TWindow : BaseBGWindow<TWindow, TManager, TBGManager>
                                                                        where TManager : BaseBGWindowManager<TWindow, TManager, TBGManager>
                                                                        where TBGManager : MonoBehaviour
{
    protected TWindow _window;
    protected TBGManager _bgManager;


    public virtual void InitManager(TBGManager bgManager)
    {
        _bgManager = bgManager;
    }

}
