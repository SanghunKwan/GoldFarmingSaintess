using GFSManagers;
using GFSUtilities.UI;
using Unity.Collections;
using UnityEngine;

public class PlayersUI : MonoBehaviour
{
    [SerializeField] Transform _slotParent;

    public PlayersUISlot[] _slots { get; private set; }

    public void InitUI(in FixedString512Bytes nicknames, out string[] nicks, bool isDataShow = true)
    {
        GameManager manager = GameManager.Instance;
        ColorScriptableObject colorData = manager._PlayerColorScriptableObject;
        nicks = nicknames.ToString().Split(' ');

        int length = nicks.Length;
        _slots = new PlayersUISlot[length];
        for (int i = 0; i < length; i++)
        {
            GameObject go = manager.InstantiateResourcePrefab(UIResourceType.PlayersUI_Slot, _slotParent);
            _slots[i] = go.GetComponent<PlayersUISlot>();
            _slots[i].InitSlot(nicks[i], colorData._color[i]);
            _slots[i].SetShowData(isDataShow);
        }
    }

}
