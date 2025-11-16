using UnityEngine;


[RequireComponent(typeof(GraphicColorController))]
public abstract class BaseBGWindow : MonoBehaviour
{
    protected GraphicColorController _controller;

    public virtual void InitWindow()
    {
        _controller = GetComponent<GraphicColorController>();
        _controller.HideAllColor(0);
    }

    public abstract void FadeIn();
    public abstract void FadeOut();
}
