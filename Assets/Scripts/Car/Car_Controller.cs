using System;
using UnityEngine;
using UnityEngine.AI;
using static Enums;

public enum DriveType { FrontWheelDrive, RearWheelDrive, AllWheelDrive }

[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(Car_HealthController))]
[RequireComponent(typeof(Car_Interaction))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
public class Car_Controller : MonoBehaviour
{
    public Car_Sounds CarSounds { get; private set; }

    public Rigidbody Rb { get; private set; }
    private PlayerControls controls;
    private float moveInput;
    private float steerInput;

    [SerializeField] private LayerMask whatIsGround;
    public bool CarActive { get; private set; }

    public float Speed;

    [Range(30, 60)]
    [SerializeField] private float turnSensetivity = 30;
    [Header("Car Settings")]
    [SerializeField] private DriveType driveType;
    [SerializeField] private Transform centerOfMass;
    [Range(350, 1000)]
    [SerializeField] private float carMass = 400;
    [Range(20, 80)]
    [SerializeField] private float wheelsMass = 30;
    [Range(.5f, 2f)]
    [SerializeField] private float frontWheelTraction = 1;
    [Range(.5f, 2f)]
    [SerializeField] private float backWheelTraction = 1;

    [Header("Engine Settings")]
    [SerializeField] private float currentSpeed;
    [Range(7, 12)]
    [SerializeField] private float maxSpeed = 7;
    [Range(.5f, 10)]
    [SerializeField] private float accleerationSpeed = 2;
    [Range(1500, 5000)]
    [SerializeField] private float motorForce = 1500f;

    [Header("Brakes Settings")]
    [Range(0, 10)]
    [SerializeField] private float frontBrakesSensetivity = 5;
    [Range(0, 10)]
    [SerializeField] private float backBrakesSensetivity = 5;
    [Range(4000, 6000)]
    [SerializeField] private float brakePower = 5000;
    private bool isBraking;

    [Header("Drift Settings")]
    [Range(0, 1)]
    [SerializeField] private float frontDriftFactor = .5f;
    [Range(0, 1)]
    [SerializeField] private float backDriftFactor = .5f;
    [SerializeField] private float driftDuration = 1f;
    private float driftTimer;
    private bool isDrifting;
    private bool canEmitTrasils = true;


    private Car_Wheel[] wheels;
    private UI ui;
    private void Start()
    {
        Rb = GetComponent<Rigidbody>();
        wheels = GetComponentsInChildren<Car_Wheel>();
        CarSounds = GetComponent<Car_Sounds>();

        ui = UI.Instance;

        controls = ControlsManager.Instance.controls;
        //ControlsManager.instance.SwitchToCarControls();

        AssignInputEvents();
        SetupDefaultValues();
        ActivateCar(false);
    }

    private void SetupDefaultValues()
    {
        Rb.centerOfMass = centerOfMass.localPosition;
        Rb.mass = carMass;

        foreach (var wheel in wheels)
        {
            wheel.Cd.mass = wheelsMass;

            if (wheel.AxelType == AxelType.Front)
                wheel.SetDefaultStiffnes(frontWheelTraction);

            if (wheel.AxelType == AxelType.Back)
                wheel.SetDefaultStiffnes(backWheelTraction);
        }

    }

    private int lastDisplayedSpeed = -1;

    private void Update()
    {
        if (CarActive == false)
            return;

        Speed = Rb.linearVelocity.magnitude;

        int displaySpeed = Mathf.RoundToInt(Speed * 10);
        if (displaySpeed != lastDisplayedSpeed)
        {
            lastDisplayedSpeed = displaySpeed;
            ui.InGameUI.UpdateSpeedText(displaySpeed + "km/h");
        }

        driftTimer -= Time.deltaTime;

        if (driftTimer < 0)
            isDrifting = false;
    }

    private void FixedUpdate()
    {
        if (CarActive == false)
            return;

        ApplyTrailsOnGround();
        ApplyAnimationToWheels();
        ApplyDrive();
        ApplySteering();
        ApplyBrakes();
        ApplySpeedLimit();

        if (isDrifting)
            ApplyDrift();
        else
            StopDrift();
    }

    private void ApplyDrive()
    {
        currentSpeed = moveInput * accleerationSpeed * Time.deltaTime;

        float motorTorqueValue = motorForce * currentSpeed;

        foreach (var wheel in wheels)
        {
            if (driveType == DriveType.FrontWheelDrive)
            {
                if (wheel.AxelType == AxelType.Front)
                    wheel.Cd.motorTorque = motorTorqueValue;
            }
            else if (driveType == DriveType.RearWheelDrive)
            {
                if (wheel.AxelType == AxelType.Back)
                    wheel.Cd.motorTorque = motorTorqueValue;
            }
            else
            {
                wheel.Cd.motorTorque = motorTorqueValue;
            }
        }
    }

    private void ApplySpeedLimit()
    {
        if (Rb.linearVelocity.magnitude > maxSpeed)
            Rb.linearVelocity = Rb.linearVelocity.normalized * maxSpeed;
    }

    private void ApplySteering()
    {
        foreach (var wheel in wheels)
        {
            if (wheel.AxelType == AxelType.Front)
            {
                float targetSteerAngle = steerInput * turnSensetivity;
                wheel.Cd.steerAngle = Mathf.Lerp(wheel.Cd.steerAngle, targetSteerAngle, .5f);
            }
        }
    }

    private void ApplyBrakes()
    {

        foreach (var wheel in wheels)
        {
            bool frontBrakes = wheel.AxelType == AxelType.Front;
            float brakeSensetivity = frontBrakes ? frontBrakesSensetivity : backBrakesSensetivity;

            float newBrakeTorque = brakePower * brakeSensetivity * Time.deltaTime;
            float currentBrakeTorque = isBraking ? newBrakeTorque : 0;

            wheel.Cd.brakeTorque = currentBrakeTorque;
        }
    }

    private void ApplyDrift()
    {
        foreach (var wheel in wheels)
        {
            bool frontWheel = wheel.AxelType == AxelType.Front;
            float driftFactor = frontWheel ? frontDriftFactor : backDriftFactor;

            WheelFrictionCurve sidewaysFriction = wheel.Cd.sidewaysFriction;

            sidewaysFriction.stiffness *= (1 - driftFactor);
            wheel.Cd.sidewaysFriction = sidewaysFriction;
        }
    }

    private void StopDrift()
    {
        foreach (var wheel in wheels)
        {
            wheel.RestoreDefaultStiffnes();
        }
    }


    private void ApplyAnimationToWheels()
    {
        foreach (var wheel in wheels)
        {
            Quaternion rotation;
            Vector3 position;

            wheel.Cd.GetWorldPose(out position, out rotation);

            if (wheel.Model != null)
            {
                wheel.Model.transform.position = position;
                wheel.Model.transform.rotation = rotation;
            }
        }
    }

    private void ApplyTrailsOnGround()
    {
        if (!canEmitTrasils)
            return;

        foreach (var wheel in wheels)
        {
            WheelHit hit;
            if (wheel.Cd.GetGroundHit(out hit))
            {
                if (whatIsGround == (whatIsGround | (1 << hit.collider.gameObject.layer)))
                {
                    wheel.Trail.emitting = true;
                }
                else
                {
                    wheel.Trail.emitting = false;
                }
            }
            else
            {
                wheel.Trail.emitting = false;
            }
        }
    }

    public void ActivateCar(bool activate)
    {
        CarActive = activate;

        if (CarSounds != null)
            CarSounds.ActivateCarSFX(activate);

        if (!activate)
            Rb.constraints = RigidbodyConstraints.FreezeAll;
        else
            Rb.constraints = RigidbodyConstraints.None;
    }

    public void BrakeTheCar()
    {
        canEmitTrasils = false;

        foreach (var wheel in wheels)
        {
            wheel.Trail.emitting = false;
        }

        Rb.linearDamping = 1;

        motorForce = 0;
        isDrifting = true;
        frontDriftFactor = .9f;
        backDriftFactor = .9f;
    }

    private void AssignInputEvents()
    {
        controls.Car.Movement.performed += ctx =>
        {
            Vector2 input = ctx.ReadValue<Vector2>();

            moveInput = input.y;
            steerInput = input.x;
        };

        controls.Car.Movement.canceled += ctx =>
        {
            moveInput = 0;
            steerInput = 0;
        };

        controls.Car.Brake.performed += ctx =>
        {
            isBraking = true;
            isDrifting = true;
            driftTimer = driftDuration;
        };
        controls.Car.Brake.canceled += ctx => isBraking = false;



        controls.Car.CarExit.performed += ctx => GetComponent<Car_Interaction>().GetOutOfTheCar();
    }

    [ContextMenu("Focus camera and enable")]
    public void TestThisCar()
    {
        ActivateCar(true);
        CameraManager.Instance.ChangeCameraTarget(transform, 40);
    }
}
