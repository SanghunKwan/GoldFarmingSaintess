using GFSManagers;
using GFSUtilities.UI;
using UnityEngine;

public class PlayersUI : MonoBehaviour
{
    [SerializeField] Transform _slotParent;

    public PlayersUISlot[] _slots { get; private set; }


    public void InitUI(in string nicknames, int nameIndex, out string[] nicks, in Color[] colors, bool isDataShow = true)
    {
        GameManager manager = GameManager.Instance;
        nicks = nicknames.Split(' ');

        int length = nicks.Length;
        _slots = new PlayersUISlot[length];
        for (int i = 0; i < length; i++)
        {
            GameObject go = manager.InstantiateResourcePrefab(UIResourceType.PlayersUI_Slot, _slotParent);
            _slots[i] = go.GetComponent<PlayersUISlot>();
            _slots[i].InitSlot(nicks[i], colors[i]);
            _slots[i].SetShowData(isDataShow);

            if (i == nameIndex - 1)
                _slots[i].SetInteractive(manager.InstantiateResourcePrefab(UIResourceType.ShowEmotion, _slots[i]._SelectorTransform));
        }
    }

    public void DisableCountOver(int playerCount)
    {
        for (int i = playerCount; i < _slots.Length; i++)
        {
            _slots[i].gameObject.SetActive(false);
        }
    }

}
