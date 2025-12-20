using GFSUtilities;
using UnityEngine;
using TMPro;
using GFSUtilities.UI;
using System.Collections;
using GFSManagers;
using System;

public class SettlementWindow : BaseBGWindow<SettlementWindow, SettlementManager, BGManager>
{
    IEnumerator _ienum;
    float _paceControl;

    public event Action FadeEvent;

    [SerializeField] TextMeshProUGUI[] _variablesText;
    [SerializeField] TextMeshProUGUI[] _calculatedText;

    [Space(20)]
    [SerializeField] GameObject _button;

    public override void InitWindow(SettlementManager manager)
    {
        base.InitWindow(manager);
        gameObject.SetActive(false);
        _button.SetActive(false);
    }

    #region Action
    public override void FadeIn()
    {
        gameObject.SetActive(true);
        _anim.SetTrigger(UIHashID.t_FadeIn);


        StartCoroutine(GFSManager.WaitForSecond(1, ShowDetails));
    }
    public override void FadeOut()
    {
        _button.SetActive(false);
        _controller.HideAllColor(0.2f);
        StartCoroutine(GFSManager.WaitForSecond(0.2f, () => _anim.SetTrigger(UIHashID.t_FadeOut)));
        StartCoroutine(GFSManager.WaitForSecond(1, () => gameObject.SetActive(false)));
    }
    public void ShowDetails()
    {
        _ienum = ShowDetailsCoroutine();
        _paceControl = 1;
        StartCoroutine(_ienum);
    }
    IEnumerator ShowDetailsCoroutine()
    {
        _controller.FadeGraphicAtOnce((int)SettlementGraphicGroupType.Title, 1, 0.5f * _paceControl);
        yield return new WaitForSeconds(0.5f * _paceControl);

        _controller.FadeGraphicAtOnce((int)SettlementGraphicGroupType.VariableNames, 1, 0.5f * _paceControl);
        yield return new WaitForSeconds(0.5f * _paceControl);
        _controller.FadeGraphicAtOnce((int)SettlementGraphicGroupType.VariableValues, 1, 0.2f * _paceControl);
        yield return new WaitForSeconds(1 * _paceControl);

        _controller.FadeGraphicInOrder((int)SettlementGraphicGroupType.CalculatedNames, 1, 0.5f * _paceControl, 0.1f * _paceControl);
        yield return new WaitForSeconds(0.5f * _paceControl);
        _controller.FadeGraphicInOrder((int)SettlementGraphicGroupType.CalculatedValues, 1, 0.2f * _paceControl, 0.4f * _paceControl);
        yield return new WaitForSeconds(2.3f * _paceControl);

        _controller.FadeGraphicAtOnce((int)SettlementGraphicGroupType.Line, 1, 0.2f * _paceControl);
        yield return new WaitForSeconds(0.5f * _paceControl);

        _controller.FadeGraphicAtOnce((int)SettlementGraphicGroupType.ResultName, 1, 0.4f * _paceControl);
        yield return new WaitForSeconds(0.5f * _paceControl);

        _controller.FadeGraphicAtOnce((int)SettlementGraphicGroupType.ResultValue, 1, 0.2f * _paceControl);
        yield return new WaitForSeconds(0.3f * _paceControl);

        _button.SetActive(true);
        FadeEvent?.Invoke();
        FadeEvent = null;
    }
    #endregion Action
    #region ValueAllocation
    public void SetValues(in string text, SettlementVariableType type)
     => _variablesText[(int)type].text = text;
    public void SetValues(in string text, SettlementCalculatedType type)
    => _calculatedText[(int)type].text = text;
    #endregion ValueAllocation
    void EndSettlement()
    {
        _manager.HideWindow();
        _manager.SendWait();

    }
    void MakePaceFast()
    {
        _paceControl = 0.2f;
    }
    #region OnEvent
    public void OnClickButton()
    {
        EndSettlement();
    }
    public void OnClickWindowBG()
    {
        MakePaceFast();
    }
    #endregion OnEvent
}
