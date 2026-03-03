using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MissionSelectionButton : UI_Button
{
    private UI_MissionSelection missionUI;
    [SerializeField] private Mission mission;

    private void OnValidate()
    {
        if (mission != null)
        {
            gameObject.name = "Button - Select Mission: " + mission.MissionName;

            if (buttonText == null)
                buttonText = GetComponentInChildren<TextMeshProUGUI>();

            if (buttonText != null)
                buttonText.text = mission.MissionName;
        }
    }

    public override void Start()
    {
        base.Start();

        missionUI = GetComponentInParent<UI_MissionSelection>();

        if (buttonText != null && mission != null)
            buttonText.text = mission.MissionName;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (missionUI != null && mission != null)
            missionUI.UpdateMissionDescription(mission.MissionDescription);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (missionUI != null)
            missionUI.UpdateMissionDescription("Choose a mission");
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        if (mission != null)
            MissionManager.Instance.SetCurrentMission(mission);
    }
}