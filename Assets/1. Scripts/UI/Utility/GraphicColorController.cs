using UnityEngine;
using UnityEngine.UI;
using System;
using GFSUtilities;


public class GraphicColorController : MonoBehaviour
{
    [SerializeField] GraphicArray[] _arrays;

    public void HideAllColor(float duration)
    {
        for (int i = 0; i < _arrays.Length; i++)
        {
            FadeGraphicAtOnce(i, 0, duration);
        }
    }

    public void FadeGraphicAtOnce(int arrayIndex, float colora, float duration)
    {
        foreach (var item in _arrays[arrayIndex]._graphics)
        {
            item.CrossFadeAlpha(colora, duration, false);
        }
    }

    public void FadeGraphicInOrder(int arrayIndex, float colora, float duration, float waitTime)
    {
        int length = _arrays[arrayIndex]._graphics.Length;

        StartCoroutine(GFSManager.ActionInOrder(length, (i) =>
            _arrays[arrayIndex]._graphics[i].CrossFadeAlpha(colora, duration, false),
            duration + waitTime));
    }


    #region VariableClass
    [Serializable]
    class GraphicArray
    {
        public MaskableGraphic[] _graphics;
    }
    #endregion VariableClass
}
