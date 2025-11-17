using UnityEngine;

[CreateAssetMenu(fileName = "SpriteScriptableObject", menuName = "Scriptable Objects/SpriteScriptableObject")]
public class SpriteScriptableObject : ScriptableObject
{
    public Sprite[] _sprites;
}
