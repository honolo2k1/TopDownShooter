using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;


[RequireComponent(typeof(WheelCollider))]
public class Car_Wheel : MonoBehaviour
{
    public AxelType AxelType;
    public WheelCollider Cd { get; private set; }
    public TrailRenderer Trail { get; private set; }
    public GameObject Model;

    private float defaultSideStiffnes;

    private void Awake()
    {
        Cd = GetComponent<WheelCollider>();
        Trail = GetComponentInChildren<TrailRenderer>();

        Trail.emitting = false;

        if(Model == null)
            Model = GetComponentInChildren<MeshRenderer>().gameObject;
    }

    public void SetDefaultStiffnes(float newValue)
    {
        defaultSideStiffnes = newValue;
        RestoreDefaultStiffnes();
    }

    public void RestoreDefaultStiffnes()
    {
        WheelFrictionCurve sidewayFriction = Cd.sidewaysFriction;

        sidewayFriction.stiffness = defaultSideStiffnes;
        Cd.sidewaysFriction = sidewayFriction;
    }
}
