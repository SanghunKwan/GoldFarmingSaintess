using GFSManagers;
using System;
using System.Collections;
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
                case MessageBoxType.Time:
                    SetButtons(0);
                    TextMeshProUGUI addedText = Instantiate(_text, transform);
                    _text.alignment = TextAlignmentOptions.Top;
                    addedText.alignment = TextAlignmentOptions.Center;
                    addedText.fontSize = 50;
                    addedText.rectTransform.anchoredPosition -= new Vector2(0, 30);
                    StartCoroutine(CheckTimer(3, addedText));
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
            SetText(_textData.text[(int)MessageBoxType.Max + index]);
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
        IEnumerator CheckTimer(int num, TextMeshProUGUI text)
        {
            int time = num;
            while (time > 0)
            {
                text.text = time.ToString();
                yield return new WaitForSeconds(1);
                --time;
            }
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