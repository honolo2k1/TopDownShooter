using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Defence Mission", menuName = "Mission/Last Defence Mission")]
public class Mission_LastDefence : Mission
{
    public bool DefenceBegun = false;

    [Header("Cooldown And Duration")]
    public float DefenceDuration = 120;
    private float defenceTimer;
    public float WaveCooldown = 15;
    private float waveTimer;

    [Header("Respawn Details")]
    public int AmountOfRespawnPoints = 2;
    public List<Transform> RespawnPoints;
    private Vector3 defencePoint;
    [Space]

    public int EnemiesPerWave;
    public GameObject[] PossibleEnemies;

    private string defenceTimerText;
    private void OnEnable()
    {
        DefenceBegun = false;
    }
    public override void StartMission()
    {
        defencePoint = Object.FindFirstObjectByType<MissionEnd_Trigger>().transform.position;
        RespawnPoints = new List<Transform>(ClosetPoints(AmountOfRespawnPoints));

        UI.Instance.InGameUI.UpdateMissionUI("Get to the evacuation point.");
    }
    public override bool MissionCompleted()
    {
        if (!DefenceBegun)
        {
            StartDefenceEvent();
            return false;
        }
        return defenceTimer < 0;
    }
    private int lastDisplayedSecond = -1;

    public override void UpdateMission()
    {
        if (!DefenceBegun) return;

        waveTimer -= Time.deltaTime;

        if (defenceTimer > 0)
        {
            defenceTimer -= Time.deltaTime;
        }

        if (waveTimer < 0)
        {
            CreateNewEnemies(EnemiesPerWave);
            waveTimer = WaveCooldown;
        }

        int currentSecond = Mathf.CeilToInt(defenceTimer);
        if (currentSecond != lastDisplayedSecond)
        {
            lastDisplayedSecond = currentSecond;
            defenceTimerText = System.TimeSpan.FromSeconds(defenceTimer).ToString("mm':'ss");

            string missionText = "Defend yourself till plane ready to take off.";
            string missionDetails = "Time left: " + defenceTimerText;

            UI.Instance.InGameUI.UpdateMissionUI(missionText, missionDetails);
        }
    }
    private void StartDefenceEvent()
    {
        waveTimer = 0.5f;
        defenceTimer = DefenceDuration;
        DefenceBegun = true;
    }
    private void CreateNewEnemies(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            int randomEnemyIndex = Random.Range(0, PossibleEnemies.Length);
            int randomRespawnIndex = Random.Range(0, RespawnPoints.Count);

            Transform randomRespawnPoint = RespawnPoints[randomRespawnIndex];
            GameObject randomEnemy = PossibleEnemies[randomEnemyIndex];

            randomEnemy.GetComponent<Enemy>().AggressionRange = 100;

            ObjectPool.Instance.GetObject(randomEnemy, randomRespawnPoint);
        }
    }
    private List<Transform> ClosetPoints(int amount)
    {
        List<Transform> closetPoints = new();

        List<MissionObject_EnemyRespawnPoint> allPoints = new(
             Object.FindObjectsByType<MissionObject_EnemyRespawnPoint>(FindObjectsInactive.Exclude, FindObjectsSortMode.None)
         );

        while (closetPoints.Count < amount && allPoints.Count > 0)
        {
            float shortestDistance = float.MaxValue;
            MissionObject_EnemyRespawnPoint closetPoint = null;

            foreach (var point in allPoints)
            {
                float distance = Vector3.Distance(point.transform.position, defencePoint);
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    closetPoint = point;
                }
            }

            if (closetPoint != null)
            {
                closetPoints.Add(closetPoint.transform);
                allPoints.Remove(closetPoint);
            }
        }
        return closetPoints;
    }
}
