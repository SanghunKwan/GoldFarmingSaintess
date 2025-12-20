using GFSManagers;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FloatingImage : MonoBehaviour
{
    [SerializeField] Image _image;

    Vector3 _direction;

    public event Action ArriveEvent;
    public void InitImage(Vector3 destination, float time, in Sprite sprite)
    {
        _image.sprite = sprite;
        _direction = (destination - transform.position) / time;
        Destroy(gameObject, time);
    }

    private void Update()
    {
        transform.position += _direction * Time.deltaTime;
    }

    private void OnDestroy()
    {
        ArriveEvent?.Invoke();
    }
}
