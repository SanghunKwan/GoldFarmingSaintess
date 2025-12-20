using GFSManagers;
using GFSUtilities;
using UnityEngine;

public class FollowCamEffect : MonoBehaviour
{
    Camera _cam;

    ParticleSystem _camEffect;
    HPManager _hpManager;

    public void InitEffect(GameObject camEffectPrefab, HPManager hpManager)
    {
        _cam = Camera.main;
        _hpManager = hpManager;

        _camEffect = Instantiate(camEffectPrefab).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        _camEffect.transform.position = _hpManager.WorldFollowUI(transform.position);
        //_camEffect.transform.position = _cam.ScreenToWorldPoint(transform.position + Vector3.forward);
    }

    private void OnDestroy()
    {
        _camEffect.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        Destroy(_camEffect.gameObject, _camEffect.main.startLifetime.constant);
    }
}
