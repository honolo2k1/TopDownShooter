using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System;

public class ZoneLimitation : MonoBehaviour
{

    private ParticleSystem[] lines;
    private BoxCollider zoneCollider;

    private void Start()
    {
        GetComponent<MeshRenderer>().enabled = false;
        zoneCollider = GetComponent<BoxCollider>();
        lines = GetComponentsInChildren<ParticleSystem>();
        ActiveWall(false);
    }

    private void ActiveWall(bool active)
    {
        foreach (var line in lines)
        {
            if (active)
            {
                line.Play();
            }
            else
            {
                line.Stop();
            }
        }
        zoneCollider.isTrigger = !active;
    }
    private async UniTaskVoid WallActiveCoroutine()
    {
        ActiveWall(true);
        await UniTask.Delay(TimeSpan.FromSeconds(1), cancellationToken: this.GetCancellationTokenOnDestroy());
        ActiveWall(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        WallActiveCoroutine().Forget();
    }
}
