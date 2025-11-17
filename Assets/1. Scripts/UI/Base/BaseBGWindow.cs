using UnityEngine;


[RequireComponent(typeof(GraphicColorController))]
public abstract class BaseBGWindow<TWindow, TManager> : MonoBehaviour where TWindow : BaseBGWindow<TWindow, TManager>
                                                                      where TManager : BaseBGWindowManager<TWindow, TManager>
{
    protected GraphicColorController _controller;
    protected TManager _manager;


    public virtual void InitWindow(TManager manager)
    {
        _controller = GetComponent<GraphicColorController>();
        _controller.HideAllColor(0);

        _manager = manager;
    }

    public abstract void FadeIn();
    public abstract void FadeOut();
}
