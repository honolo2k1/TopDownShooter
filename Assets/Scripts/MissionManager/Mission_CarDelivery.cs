using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Car Mission", menuName = "Mission/Car Delivery Mission")]
public class Mission_CarDelivery : Mission
{
    private bool carWasDelivered;
    public override void StartMission()
    {
        FindObjectOfType<MissionObject_CarDeliveryZone>(true).gameObject.SetActive(true);

        string missionText = "Find a functional vehicle.";
        string missionDetails = "Deliver it to the evacuation point.";

        UI.Instance.InGameUI.UpdateMissionUI(missionText, missionDetails);

        carWasDelivered = false;
        MissionObject_CarDeliver.OnCarDelivery += CarDeliveryCompleted;

        Car_Controller[] cars = FindObjectsOfType<Car_Controller>();

        foreach (Car_Controller car in cars)
        {
            car.AddComponent<MissionObject_CarDeliver>();
        }
    }

    public override bool MissionCompleted()
    {
        return carWasDelivered;
    }
    private void CarDeliveryCompleted()
    {
        carWasDelivered = true;
        MissionObject_CarDeliver.OnCarDelivery -= CarDeliveryCompleted;

        UI.Instance.InGameUI.UpdateMissionUI("Get to the evacuation point.");
    }
}
