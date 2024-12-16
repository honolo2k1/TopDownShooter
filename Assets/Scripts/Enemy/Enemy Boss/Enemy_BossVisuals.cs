using UnityEngine;

public class Enemy_BossVisuals : MonoBehaviour
{
    public Enemy_Boss Enemy;
    [SerializeField] private float ladingOffset;
    [SerializeField] private ParticleSystem landingZoneFx;
    [SerializeField] private GameObject[] trails;
    private void Awake()
    {
        Enemy = GetComponent<Enemy_Boss>();

        landingZoneFx.transform.parent = null;
        landingZoneFx.Stop();
    }

    public void EnableTrails(bool enable)
    {
        if (trails.Length <= 0)
        {
            return;
        }
        foreach (var trail in trails)
        {
            trail.SetActive(enable);
        }
    }
    public void PlaceLadingZone(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        Vector3 offset = dir.normalized * ladingOffset; 
        landingZoneFx.transform.position = target + offset;
        landingZoneFx.Clear();

        var mainModule = landingZoneFx.main;
        mainModule.startLifetime = Enemy.TravelTimeToTarget * 2;

        landingZoneFx.Play();
    }
}
