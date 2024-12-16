using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    IEnumerator WallActiveCoroutine()
    {
        ActiveWall(true);
        yield return new WaitForSeconds(1);
        ActiveWall(false);
    }
    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(WallActiveCoroutine());
    }
}
