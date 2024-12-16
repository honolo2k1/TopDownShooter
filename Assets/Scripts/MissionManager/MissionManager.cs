using UnityEngine;

public class MissionManager : MonoBehaviour
{

    public static MissionManager Instance;

    public Mission CurrentMission;

    private void Awake()
    {
        Instance = this;
    }
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
