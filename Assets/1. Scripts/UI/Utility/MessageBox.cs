using GFSManagers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GFSUtilities.UI
{
    public class MessageBox : MonoBehaviour
    {
        [SerializeField] Image _mean;
        [SerializeField] TextMeshProUGUI _text;
        [SerializeField] Transform _buttonParent;

        int _buttonCount;
        SpriteScriptableObject _spriteData;
        TextScriptableObject _textData;

        public event Action<int> OnButtonClickDispose;

        public void InitBox()
        {
            _buttonCount = _buttonParent.childCount;

            GameManager manager = GameManager.Instance;
            _spriteData = manager._UISpriteScriptableObject;
            _textData = manager._UITextScriptableObject;
            gameObject.SetActive(false);
        }
        public void SetBox(MessageBoxType type)
        {
            gameObject.SetActive(true);
            SetText(_textData.text[(int)type]);
            SetImage(type);
            switch (type)
            {
                case MessageBoxType.Proceed:
                    SetButtons(1);
                    break;
                case MessageBoxType.Alert:
                    SetButtons(1);
                    break;
                case MessageBoxType.Check:
                    SetButtons(2);
                    break;
                default:
                    break;
            }
        }
        public void SetText(in string str)
        {
            _text.text = str;
        }
        public void SetText(int index)
        {
            _text.text = _textData.text[index];
        }
        void SetButtons(int buttonCount)
        {
            for (int i = 0; i < _buttonCount; i++)
            {
                _buttonParent.GetChild(i).gameObject.SetActive(i < buttonCount);
            }
        }
        void SetImage(MessageBoxType type)
        {
            _mean.sprite = _spriteData._sprites[(int)type + (int)Unit.UnitTypes.Count * 2];
        }
        #region Event
        public void OnClickButton(int buttonIndex)
        {
            OnButtonClickDispose?.Invoke(buttonIndex);
            gameObject.SetActive(false);
            OnButtonClickDispose = null;
        }
        #endregion
    }
}