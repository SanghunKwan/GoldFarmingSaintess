using GFSManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace GFSUtilities.UI
{

    public class SettingWindow : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI _uidText;
        [SerializeField] InputField _nickNameField;

        bool _isChanged;

        LoginManager _manager;

        public void InitWindow(LoginManager manager)
        {
            _manager = manager;
            _isChanged = false;
            _uidText.text = _manager._id.ToString("D5");
            gameObject.SetActive(false);
        }

        #region Toggle
        public void ToggleWindow()
        {
            if (gameObject.activeSelf)
                QuitWindow();
            else
                ActiveWindow();
        }
        void ActiveWindow()
        {
            gameObject.SetActive(true);
            _nickNameField.text = _manager._nickName;
            _isChanged = false;
        }
        public void DisactiveWindow()
        {
            gameObject.SetActive(false);
        }
        void QuitWindow()
        {
            if (_isChanged)
                _manager.SettingCancel();
            else
                DisactiveWindow();
        }
        #endregion Toggle
        #region Event
        public void OnClickSaveButton()
        {
            if (_isChanged)
            {
                _manager.SubmitSetting(_nickNameField.text);
            }
            else
                DisactiveWindow();
        }
        public void OnClickQuitButton()
        {
            QuitWindow();
        }
        public void OnDataChanged()
        {
            _isChanged = true;
        }
        #endregion Event
    }
}