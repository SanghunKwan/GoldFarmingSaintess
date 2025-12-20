using GFSUtilities;
using GFSUtilities.Item;
using GFSUtilities.UI;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace GFSManagers
{
    public class ExplainManager : MonoBehaviour
    {
        public HPManager _hpManager { get; set; }



        ExplainWindow _window;
        ExplainData[] _explainTexts;

        public void InitManager()
        {
            GameManager manager = GameManager.Instance;

            _window = manager.InstantiatePrefab(UIType.Explain, transform).GetComponent<ExplainWindow>();
            _window.InitWindow();
            _explainTexts = manager._UITextScriptableObject.explainText;
        }

        public void CallWindow(ExplainType type, in Vector3 position)
        {
            ref readonly ExplainData data = ref _explainTexts[(int)type];
            _window.CallWindow(_hpManager.UIPosition(position));
            _window.SetText(data.title, data.details);
        }

        public void ShowAtMousPosition(ExplainType explainType, in string details, bool showButtons = false)
        {
            CallWindow(explainType, Input.mousePosition);
            _window.SetMoreDetailText(details);
            _window.SetSize(showButtons);
        }
        void SetActiveButtonCount(ItemType type)
        {
            if (type == ItemType.°á»ê)
            {
                _window.SetButtonSetActive(0, true);
                //_window.SetButtonSetActive(1, true);
            }
            else
            {
                _window.SetButtonSetActive(0, false);
                //_window.SetButtonSetActive(1, true);
            }
        }
        public void SetButtons(InventoryManager manager, int slotIndex, ItemType type)
        {
            SetActiveButtonCount(type);
            _window.ClearEvent();
            _window._UseEvent += () => manager.UseItem(slotIndex);
            _window._ThrowEvent += () => manager.RemoveItem(slotIndex);
        }
        public void HideWindow()
        {
            if (_window != null)
                _window.SetWindowActive(false);
        }
    }
}