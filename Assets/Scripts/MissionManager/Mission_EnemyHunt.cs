using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static Enums;
[CreateAssetMenu(fileName = "New Hunt Mission", menuName = "Mission/Hunt Mission")]
public class Mission_EnemyHunt : Mission
{
    public int AmountToKill = 12;
    public EnemyType EnemyType;

    private int killsToGo;
    public override void StartMission()
    {
        killsToGo = AmountToKill;
        UpdateMissionUI();

        MissionObject_HuntTarget.OnTargetKilled += EliminateTarget;

        List<Enemy> validEnemies = new List<Enemy>();

        if (EnemyType == EnemyType.Random)
        {
            validEnemies = LevelGenarator.Instance.GetEnemyList();
        }
        else
        {
            foreach (Enemy enemy in LevelGenarator.Instance.GetEnemyList())
            {
                if (enemy.EnemyType == EnemyType)
                {
                    validEnemies.Add(enemy);
                }
            }
        }

        for (int i = 0; i < AmountToKill; i++)
        {
            if (validEnemies.Count <= 0)
            {
                return;
            }
            int randomIndex = Random.Range(0, validEnemies.Count);
            validEnemies[randomIndex].AddComponent<MissionObject_HuntTarget>();
            validEnemies.RemoveAt(randomIndex);
        }
    }
    public override bool MissionCompleted()
    {
        return killsToGo <= 0;
    }

    private void EliminateTarget()
    {
        killsToGo--;
        UpdateMissionUI();

        if (killsToGo <= 0)
        {
            UI.Instance.InGameUI.UpdateMissionUI("Get to the evacuation point.");
            MissionObject_HuntTarget.OnTargetKilled -= EliminateTarget;
        }
    }

    private void UpdateMissionUI()
    {
        string missionText = "Eliminate " + AmountToKill + " enemies with signal disruptor.";
        string missionDetaisl = "Targets left: " + killsToGo;

        UI.Instance.InGameUI.UpdateMissionUI(missionText, missionDetaisl);
    }
}
