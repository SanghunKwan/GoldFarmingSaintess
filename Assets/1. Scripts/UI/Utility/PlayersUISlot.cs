using GFSUtilities.UI;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayersUISlot : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nickName;
    [SerializeField] TextMeshProUGUI _goldText;

    [SerializeField] Image _colorImage;


    [SerializeField] GraphicColorController _graphicColorController;
    [SerializeField] GameObject _dataObject;
    [SerializeField] GameObject _emotionObject;
    [SerializeField] Image _emotionImage;
    [SerializeField] TextMeshProUGUI _voteCostText;

    [Header("EmotionSelector")]
    [SerializeField] EventTrigger _clickEvent;
    [SerializeField] Transform _selectorTransform;
    [SerializeField] CanvasGroup _canvasGroup;

    IEnumerator _ienum;

    int beforeMoney;
    int _controllerLength;
    bool _emotionStop;
    bool _emotionSelectActive;

    public Vector3 _RankingPosition => _emotionObject.transform.position;
    public Transform _SelectorTransform => _selectorTransform;

    public void InitSlot(in string nickName, in Color color)
    {
        _colorImage.color = color;
        _nickName.text = nickName;
        beforeMoney = -1;
        _controllerLength = (int)PlayerUISlotGroupType.Max;

        _graphicColorController.HideAllColor(0);

        _emotionStop = false;
        _emotionSelectActive = false;
    }
    public void SetShowData(bool isOn)
    {
        _dataObject.SetActive(isOn);
    }

    public void ShowMoney(int gold)
    {
        if (gold == beforeMoney) return;

        _goldText.text = gold.ToString("N0");
    }

    public void ShowEmotion(in Sprite emotionSprite)
    {
        if (_emotionStop) return;

        _emotionImage.sprite = emotionSprite;

        if (_ienum != null)
        {
            StopCoroutine(_ienum);
            _graphicColorController.HideAllColor(0);
        }

        _graphicColorController.FadeGraphicAtOnce((int)PlayerUISlotGroupType.Emotion, 1, 0.5f);
        _ienum = ShowSpeechBubbles();
        StartCoroutine(_ienum);
    }

    IEnumerator ShowSpeechBubbles()
    {
        _graphicColorController.FadeGraphicAtOnce(0, 1, 0.5f);

        yield return new WaitForSeconds(3);
        for (int i = 0; i < _controllerLength; i++)
            _graphicColorController.FadeGraphicAtOnce(i, 0, 0.5f);
        _emotionStop = false;

        yield return new WaitForSeconds(0.5f);
        _ienum = null;
    }
    public void ShowCostVote(int cost)
    {
        if (_ienum != null)
        {
            StopCoroutine(_ienum);
            _graphicColorController.HideAllColor(0);
        }
        _voteCostText.text = cost.ToString("N0");
        _graphicColorController.FadeGraphicAtOnce((int)PlayerUISlotGroupType.CostVote, 1, 0.5f);
        _emotionStop = true;

        _ienum = ShowSpeechBubbles();
        StartCoroutine(_ienum);
    }
    public void ShowResult(int ranking)
    {
        if (_ienum != null)
        {
            StopCoroutine(_ienum);
            _graphicColorController.HideAllColor(0);
        }
        _voteCostText.text = $"{ranking}µî";
        _graphicColorController.FadeGraphicAtOnce((int)PlayerUISlotGroupType.CostVote, 1, 0.5f);
        _graphicColorController.FadeGraphicAtOnce((int)PlayerUISlotGroupType.SpeechBubble, 1, 0.5f);
        _emotionStop = true;
    }
    public void SetInteractive(GameObject emotionSelector)
    {
        emotionSelector.transform.SetParent(_selectorTransform);
        _clickEvent.enabled = true;

        emotionSelector.GetComponent<EmotionSelector>().InitSelector(this);
    }

    #region Event
    public void ToggleEmotionSelector()
    {
        _emotionSelectActive = !_emotionSelectActive;

        _canvasGroup.alpha = _emotionSelectActive ? 1 : 0;
        _canvasGroup.blocksRaycasts = _emotionSelectActive;
    }
    #endregion Event
}
