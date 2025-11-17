using GFSManagers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using GFSUtilities.UI;
using GFSUtilities.Unit;

public class UnitSelectSlot : MonoBehaviour
{
    [SerializeField] Image _portrait;
    [SerializeField] TextMeshProUGUI _unitCount;
    [SerializeField] Transform _starParentTr;


    public void SetImage(in Sprite image)
    {
        _portrait.sprite = image;
    }
    public void SetCount(int count)
    {
        _unitCount.text = count.ToString("N0");
    }
    public void MakeStars(StarCount count)
    {
        int length = (int)count;
        for (int i = 0; i < length; i++)
            GameManager.Instance.InstantiateResourcePrefab(UIResourceType.SelectStar, _starParentTr);
    }
}
