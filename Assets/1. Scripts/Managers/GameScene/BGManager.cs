using GFSUtilities;
using System;
using UnityEngine;
using UnityEngine.UI;


namespace GFSManagers
{
    public class BGManager : MonoBehaviour
    {
        Animator _anim;
        Image _bgImage;

        int _currentBGUICount;
        public int _CurrentBGUICount
        {
            get => _currentBGUICount;
            set
            {
                if (value > _currentBGUICount)
                {
                    if (_currentBGUICount == 0)
                        SetBGActive();
                }
                else
                {
                    if (value == 0)
                        SetBGDisactive();
                }

                _currentBGUICount = value;
            }
        }

        public void InitManager()
        {
            _anim = GetComponent<Animator>();
            _bgImage = GetComponent<Image>();
            _bgImage.enabled = false;
        }

        public void CallUI(float second, BaseBGWindow _window)
        {
            _CurrentBGUICount++;

            StartCoroutine(GFSManager.WaitForSecond(second, _window.FadeIn));
        }

        public void ReleaseUI()
        {
            _CurrentBGUICount--;

        }


        void SetBGActive()
        {
            _bgImage.enabled = true;
            _anim.SetBool(UIHashID.b_IsBlack, true);
        }

        void SetBGDisactive()
        {
            _anim.SetBool(UIHashID.b_IsBlack, true);

            StartCoroutine(GFSManager.WaitForSecond(0.5f, () => _bgImage.enabled = false));
        }
    }
}