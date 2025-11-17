using UnityEngine;

public abstract class BaseBGWindowManager<TWindow, TManager> where TWindow : BaseBGWindow<TWindow, TManager>
                                                            where TManager : BaseBGWindowManager<TWindow, TManager>
{
    protected TWindow _window;


}
