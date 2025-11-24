using GFSManagers;
using GFSUtilities.UI;
using Unity.Collections;
using UnityEngine;

public class PlayersUI : MonoBehaviour
{
    [SerializeField] Transform _slotParent;

    public PlayersUISlot[] _slots { get; private set; }

    public void InitUI(in FixedString512Bytes nicknames)
    {
        GameManager manager = GameManager.Instance;
        ColorScriptableObject colorData = manager._PlayerColorScriptableObject;
        var nicks = nicknames.ToString().Split(' ');

        int length = nicks.Length;
        _slots = new PlayersUISlot[length];
        for (int i = 0; i < length; i++)
        {
            GameObject go = manager.InstantiateResourcePrefab(UIResourceType.PlayersUI_Slot);
            _slots[i] = go.GetComponent<PlayersUISlot>();
            _slots[i].SetData(nicks[i], colorData._color[i]);
        }


    }
}
