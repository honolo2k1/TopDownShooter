using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Timer Mission", menuName = "Mission/Timer Mission")]
public class Mission_Timer : Mission
{
    public float TimeSet;
    private float currentTime;
    public override void StartMission()
    {
        currentTime = TimeSet;
    }
    public override void UpdateMission()
    {
        currentTime -= Time.deltaTime;
        if (currentTime < 0)
        {
            //GameManager.Instance.GameOver();
        }
        string timeText = System.TimeSpan.FromSeconds(currentTime).ToString("mm':'ss");

        string missionText = "Get to evacuation point before plane take off.";
        string missionDetails = "Time left: " + timeText;

        UI.Instance.InGameUI.UpdateMissionUI(missionText, missionDetails);
    }

    public override bool MissionCompleted()
    {
        return currentTime > 0;
    }
}
