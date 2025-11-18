using GFSUtilities;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


namespace GFSManagers
{
    public class BGManager : MonoBehaviour
    {
        Animator _anim;
        Image _bgImage;

        int _currentBGUICount;

        IEnumerator _ienum;


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

        public void CallUI<TWindow, TManager>(float second, BaseBGWindow<TWindow, TManager> window)
                                                                where TWindow : BaseBGWindow<TWindow, TManager>
                                                                where TManager : BaseBGWindowManager<TWindow, TManager>
        {
            _CurrentBGUICount++;

            StartCoroutine(GFSManager.WaitForSecond(second, window.FadeIn));
        }

        public void ReleaseUI()
        {
            _CurrentBGUICount--;

        }


        void SetBGActive()
        {
            if (_ienum != null)
            {
                StopCoroutine(_ienum);
                _ienum = null;
            }

            _bgImage.enabled = true;
            _anim.SetBool(UIHashID.b_IsBlack, true);
        }

        void SetBGDisactive()
        {
            _anim.SetBool(UIHashID.b_IsBlack, false);

            _ienum = GFSManager.WaitForSecond(2f, () =>
            {
                _bgImage.enabled = false;
                _ienum = null;
            });
            StartCoroutine(_ienum);
        }
    }
}