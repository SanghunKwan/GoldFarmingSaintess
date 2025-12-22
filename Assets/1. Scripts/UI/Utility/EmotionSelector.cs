using GFSManagers;
using GFSUtilities.ResourcesData;
using UnityEngine;

public class EmotionSelector : MonoBehaviour
{
    PlayersUISlot _parentSlot;


    public void InitSelector(PlayersUISlot slot)
    {
        _parentSlot = slot;
    }


    public void ClickEmotion(int emotion)
        => SelectEmotion((EmotionType)emotion);

    void SelectEmotion(EmotionType type)
    {
        GameSceneManager.Instance.SetEmotion(type);
        _parentSlot.ToggleEmotionSelector();
    }
}
