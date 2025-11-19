using GFSManagers;
using GFSUtilities;
using GFSUtilities.UI;
using System.Collections;
using TMPro;
using UnityEngine;

public class TurnWindow : BaseBGWindow<TurnWindow, TurnManager, BGManager>
{

    [SerializeField] TextMeshProUGUI _currentTurnText;
    [SerializeField] TextMeshProUGUI _maxTurntext;


    public override void InitWindow(TurnManager manager)
    {
        base.InitWindow(manager);
        gameObject.SetActive(false);
    }
    public void SetMaxTurn(int num)
    {
        _maxTurntext.text = num.ToString();
        SetCurrentTurn(_manager._currentTurn);
        _controller.HideAllColor(0);
    }
    public void SetCurrentTurn(int num)
    {
        _currentTurnText.text = num.ToString();
    }

    public override void FadeIn()
    {
        gameObject.SetActive(true);
        _anim.SetTrigger(UIHashID.t_FadeIn);
        StartCoroutine(ShowDetailsCoroutine());
    }
    IEnumerator ShowDetailsCoroutine()
    {
        _controller.FadeGraphicAtOnce((int)TurnGraphicGroupType.BG, 1, 0.4f);
        yield return new WaitForSeconds(0.4f);

        _controller.FadeGraphicAtOnce((int)TurnGraphicGroupType.TurnText, 1, 0.3f);
        yield return new WaitForSeconds(0.1f);

        _controller.FadeGraphicInOrder((int)TurnGraphicGroupType.NoneChangeSubText, 1, 0.2f, 0.1f);
        yield return new WaitForSeconds(0.6f);

        _controller.FadeGraphicAtOnce((int)TurnGraphicGroupType.CurrentTurnText, 1, 0.5f);
        yield return new WaitForSeconds(0.5f);

        _controller.FadeGraphicAtOnce((int)TurnGraphicGroupType.ShiningEffect, 0.7f, 0.75f);
        yield return new WaitForSeconds(0.75f);

        _controller.FadeGraphicAtOnce((int)TurnGraphicGroupType.ShiningEffect, 0.3f, 0.75f);
        yield return new WaitForSeconds(0.75f);

        _controller.FadeGraphicAtOnce((int)TurnGraphicGroupType.ShiningEffect, 0.7f, 0.75f);
        yield return new WaitForSeconds(0.75f);

        _manager.FadeOut();
    }

    public override void FadeOut()
    {
        _anim.SetTrigger(UIHashID.t_FadeOut);
        _controller.HideAllColor(0.3f);
        StartCoroutine(GFSManager.WaitForSecond(0.6f, () => gameObject.SetActive(false)));
    }






}
