using GFSManagers;
using GFSUtilities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BiddingWindow : BaseBGWindow<BiddingWindow, BiddingManager, NoneBGManager>
{
    [SerializeField] TextMeshProUGUI _marketPriceText;
    [SerializeField] TextMeshProUGUI _currentCostText;
    [SerializeField] Image _itemImage;


    public Vector3 _ItemImagePosition => _itemImage.transform.position;

    public override void InitWindow(BiddingManager manager)
    {
        base.InitWindow(manager);

        gameObject.SetActive(false);

        _manager.CurrentCostChangeEvent += ChangeCurrentCost;
    }

    public override void FadeIn()
    {
        gameObject.SetActive(true);
        _anim.SetTrigger(UIHashID.t_FadeIn);
        _itemImage.CrossFadeAlpha(1, 0.3f, false);
    }

    public override void FadeOut()
    {
        _anim.SetTrigger(UIHashID.t_FadeOut);
        _itemImage.CrossFadeAlpha(0, 0.5f, false);
        StartCoroutine(GFSManager.WaitForSecond(0.8f, () => gameObject.SetActive(false)));
    }

    public void SetData(int cost, in Sprite image)
    {
        _marketPriceText.text = cost.ToString("N0");
        _itemImage.sprite = image;
        _itemImage.CrossFadeAlpha(0, 0, false);
    }

    public void AfterSent()
    {
        _anim.SetTrigger(UIHashID.t_Sent);
    }


    #region Event
    void ChangeCurrentCost(int cost)
    {
        _currentCostText.text = cost.ToString("N0");
    }
    public void OnClickCostChange(int costChanged)
    {
        _manager._CurrentCost += costChanged;
    }

    public void OnClickSubmit()
    {
        _manager.SubmitCost();
    }
    public void OnClickMarketPrice()
    {
        _manager.SetMarketPrice();
    }

    public void OnClickCurrentPriceSlot()
    {
        _manager.CallBiddingExplain();
    }

    public void OnClickCurrentItemSlot()
    {
        _manager.CallItemExplain();
    }
    #endregion Event
}
