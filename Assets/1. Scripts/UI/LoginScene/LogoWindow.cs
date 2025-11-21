using GFSManagers;
using System.Collections;
using UnityEngine;
using GFSUtilities.UI;

public class LogoWindow : BaseBGWindow<LogoWindow, LogoManager, BGManager>
{

    public override void InitWindow(LogoManager manager)
    {
        base.InitWindow(manager);
        gameObject.SetActive(false);
        _controller.HideAllColor(0);
    }


    public override void FadeIn()
    {
        gameObject.SetActive(true);
        StartCoroutine(PlayLogo());
    }
    IEnumerator PlayLogo()
    {
        yield return new WaitForSeconds(0.5f);
        _controller.FadeGraphicAtOnce((int)LogoGraphicGroupType.UnityChan, 1, 1f);
        yield return new WaitForSeconds(2f);

        _controller.FadeGraphicAtOnce((int)LogoGraphicGroupType.UnityChan, 0, 1f);
        yield return new WaitForSeconds(1.5f);

        _manager.EndWindow();
    }

    public override void FadeOut()
    {
        gameObject.SetActive(false);
    }
}
