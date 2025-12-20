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
            _bgImage.raycastTarget = false;
        }

        public IEnumerator CallUI<TWindow, TManager, TBGManager>(float second, BaseBGWindow<TWindow, TManager, TBGManager> window)
                                                                where TWindow : BaseBGWindow<TWindow, TManager, TBGManager>
                                                                where TManager : BaseBGWindowManager<TWindow, TManager, TBGManager>
                                                                where TBGManager : MonoBehaviour
        {
            _CurrentBGUICount++;

            IEnumerator tempInum = GFSManager.WaitForSecond(second, window.FadeIn);
            StartCoroutine(tempInum);

            return tempInum;
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

            _anim.SetBool(UIHashID.b_IsBlack, true);
            _bgImage.raycastTarget = true;
        }

        void SetBGDisactive()
        {
            _anim.SetBool(UIHashID.b_IsBlack, false);

            _ienum = GFSManager.WaitForSecond(0.5f, () =>
            {
                _ienum = null;
                _bgImage.raycastTarget = false;
            });
            StartCoroutine(_ienum);
        }
    }
}