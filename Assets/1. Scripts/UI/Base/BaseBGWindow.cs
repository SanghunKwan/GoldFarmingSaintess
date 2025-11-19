using UnityEngine;


[RequireComponent(typeof(GraphicColorController))]
public abstract class BaseBGWindow<TWindow, TManager, TBGManager> : MonoBehaviour where TWindow : BaseBGWindow<TWindow, TManager, TBGManager>
                                                                      where TManager : BaseBGWindowManager<TWindow, TManager, TBGManager>
                                                                        where TBGManager : MonoBehaviour
{
    protected GraphicColorController _controller;
    protected TManager _manager;
    protected Animator _anim;


    public virtual void InitWindow(TManager manager)
    {
        _anim = GetComponent<Animator>();
        _controller = GetComponent<GraphicColorController>();
        _controller.HideAllColor(0);

        _manager = manager;
    }

    public abstract void FadeIn();
    public abstract void FadeOut();
}
