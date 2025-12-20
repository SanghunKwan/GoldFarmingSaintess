using GFSBattle;
using GFSUtilities.Item;
using GFSUtilities.UI;
using System.Text;
using UnityEngine;

namespace GFSManagers
{
    public class InventoryManager : BaseBGWindowManager<InventoryUI, InventoryManager, NoneBGManager>
    {
        readonly int itemIndexAdd = (int)UISpriteType.ItemCane;
        Sprite[] _spriteData;
        public Item[] _itemData { get; private set; }
        public int[] _items { get; private set; }


        public ExplainManager _explainManager { get; set; }
        public SelectManager _selectManager { get; set; }


        public override void InitManager(NoneBGManager bgManager)
        {
            base.InitManager(bgManager);
            var manager = GameManager.Instance;
            _spriteData = manager._UISpriteScriptableObject._sprites;
            _itemData = manager._ItemScriptableObject.items;

            _items = new int[2] { -1, -1 };
        }


        public void CallUI(float second)
        {
            if (_window == null)
            {
                GameObject go = GameManager.Instance.InstantiatePrefab(UIType.Inventory, _bgManager.transform);
                _window = go.GetComponent<InventoryUI>();
                _window.InitWindow(this);
            }

            _bgManager.CallUI(second, _window);

        }

        public void FadeOut()
        {
            _window.FadeOut();
        }
        public Sprite GetItemSprite(int itemIndex)
        => _spriteData[itemIndexAdd + itemIndex];

        public bool HasEmptySlot(out int index)
        {
            index = System.Array.IndexOf(_items, -1);
            return index != -1;
        }
        public Vector3 SlotPosition(int index) => _window.SlotPosition(index);

        public void SetItem(int slotIndex, int itemIndex)
        {
            _items[slotIndex] = itemIndex;
            _window.SetSlot(slotIndex, GetItemSprite(itemIndex));
        }


        public string GetItemExplain(int itemIndex)
        {
            StringBuilder sb = new StringBuilder();
            ref readonly Item item = ref _itemData[itemIndex];

            sb.Append("이름 : ");
            sb.AppendLine(item.name);

            sb.Append("분류 : ");
            sb.AppendLine(item.type.ToString());

            sb.Append("설명 : ");
            sb.AppendLine(item.description);

            return sb.ToString();
        }
        public void CallItemExplain(int slotIndex)
        {
            int itemIndex = _items[slotIndex];
            if (itemIndex == -1) return;

            _explainManager.ShowAtMousPosition(ExplainType.InventoryItem, GetItemExplain(itemIndex), true);
            _explainManager.SetButtons(this, slotIndex, _itemData[itemIndex].type);
        }

        public void UseItem(int slotIndex)
        {
            int itemIndex = _items[slotIndex];
            ref readonly var item = ref _itemData[itemIndex];

            RemoveItem(slotIndex);

            _selectManager._additionalWeightArray[item.index] *= 2;
            //index == 2인 경우 미구현.
            //index가 3 초과인 경우 함수에 접근 불가.
        }
        public void RemoveItem(int slotIndex)
        {
            _items[slotIndex] = -1;
            _window.EmptySlot(slotIndex);
        }

        public void BindBattleManager(BattleManager battleManager)
        {
            battleManager._inventoryManager = this;

            for (int i = 0; i < _items.Length; i++)
            {
                int index = _items[i];
                if (index == -1) continue;

                ref readonly var item = ref _itemData[index];

                if (item.type != ItemType.전투) continue;

                battleManager.RegisterBattleItems((ItemAutoUseType)item.index - (int)SelectWeightType.Max, i);
            }


        }
    }
}