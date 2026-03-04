using UnityEngine;

public class MissionManager : MonoSingleton<MissionManager>
{
    public Mission CurrentMission;

    private void Update()
    {
        CurrentMission?.UpdateMission();
    }
    public void SetCurrentMission(Mission mission)
    {
        CurrentMission = mission;
    }
    public void StartMission() => CurrentMission.StartMission();

    public bool MissionCompleted() => CurrentMission.MissionCompleted();
}
