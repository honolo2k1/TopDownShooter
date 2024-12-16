using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_MissionSelectionButton : UI_Button
{
    private UI_MissionSelection missionUI;
    [SerializeField] private Mission mission;
    private TextMeshProUGUI buttonText;

    private void OnValidate()
    {
        gameObject.name = "Button - Select Mission: " + mission.MissionName;
        if (buttonText != null )
            buttonText.text = mission.MissionName;
    }
    public override void Start()
    {
        base.Start();

        missionUI = GetComponentInParent<UI_MissionSelection>();
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
        if (buttonText != null )
            buttonText.text = mission.MissionName;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        missionUI.UpdateMissionDescription(mission.MissionDescription);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        missionUI.UpdateMissionDescription("Choose a mission");
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        MissionManager.Instance.SetCurrentMission(mission);
    }
}
