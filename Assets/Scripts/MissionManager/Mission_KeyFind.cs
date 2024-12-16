using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Key Mission", menuName = "Mission/Key Mission")]
public class Mission_KeyFind : Mission
{
    [SerializeField] private GameObject key;
    private bool keyFound;
    public override void StartMission()
    {
        MissionObject_Key.OnKeyPickUp += PickupKey;

        UI.Instance.InGameUI.UpdateMissionUI("Find a key-holder. Retrive the key.");

        Enemy enemy = LevelGenarator.Instance.GetRandomEnemy();
        enemy.GetComponent<Enemy_DropController>()?.GiveKey(key);
        enemy.MakeEnemyVIP();
    }

    public override bool MissionCompleted()
    {
        return keyFound;
    }

    private void PickupKey()
    {
        keyFound = true;
        MissionObject_Key.OnKeyPickUp -= PickupKey;

        UI.Instance.InGameUI.UpdateMissionUI("You've got the key! \n Get to the evacuation point.");
    }
}
