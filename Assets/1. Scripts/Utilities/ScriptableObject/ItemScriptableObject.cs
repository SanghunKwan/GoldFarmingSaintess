using GFSUtilities.Item;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemScriptableObject", menuName = "Scriptable Objects/ItemScriptableObject")]
public class ItemScriptableObject : ScriptableObject
{
    public Item[] items;
}


