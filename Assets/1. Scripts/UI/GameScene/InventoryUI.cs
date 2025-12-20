using GFSManagers;
using GFSUtilities;
using GFSUtilities.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUI : BaseBGWindow<InventoryUI, InventoryManager, NoneBGManager>
{
    readonly string imageName = "ItemImage";




    [SerializeField] Transform _inventorySlotTr;
    Image[] _itemSlots;



    public override void InitWindow(InventoryManager manager)
    {
        base.InitWindow(manager);

        int length = _manager._items.Length;

        _itemSlots = new Image[length];
        for (int i = 0; i < length; i++)
        {
            GameObject go = GameManager.Instance.InstantiateResourcePrefab(UIResourceType.ItemSlot, _inventorySlotTr);
            var trigger = go.GetComponent<EventTrigger>();
            int tempIndex = i;
            trigger.triggers[0].callback.AddListener((data) => OnClickItemSlot(tempIndex));
            _itemSlots[i] = go.transform.Find(imageName).GetComponent<Image>();
            EmptySlot(i);
        }
        gameObject.SetActive(false);

    }


    public override void FadeIn()
    {
        gameObject.SetActive(true);
        _anim.SetTrigger(UIHashID.t_FadeIn);
    }

    public override void FadeOut()
    {
        _anim.SetTrigger(UIHashID.t_FadeOut);
        StartCoroutine(GFSManager.WaitForSecond(0.3f, () => gameObject.SetActive(false)));
    }


    public void SetSlot(int itemIndex, in Sprite itemSprite)
    {
        Image tempImage = _itemSlots[itemIndex];
        tempImage.sprite = itemSprite;
        tempImage.CrossFadeAlpha(1, 0, false);
    }
    public void EmptySlot(int itemIndex)
    {
        _itemSlots[itemIndex].CrossFadeAlpha(0, 0, false);
    }
    public Vector3 SlotPosition(int slotIndex)
        => _itemSlots[slotIndex].transform.position;

    #region Event
    public void OnClickItemSlot(int slotIndex)
    {
        _manager.CallItemExplain(slotIndex);
    }
    #endregion Event
}
