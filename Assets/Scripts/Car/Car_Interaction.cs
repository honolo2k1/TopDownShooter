using UnityEngine;

public class Car_Interaction : Interacable
{
    private Car_HealthController carHealthController;
    private Car_Controller carController;
    private Transform player;

    private float defaultPlayerScale;


    [Header("Exit details")]
    [SerializeField] private float exitCheckRadius = .2f;
    [SerializeField] private Transform[] exitPoints;
    [SerializeField] private LayerMask whatToIngoreForExit;

    private void Start()
    {
        carHealthController = GetComponent<Car_HealthController>();
        carController = GetComponent<Car_Controller>();
        player = GameManager.Instance.Player.transform;

        foreach (var point in exitPoints)
        {
            point.GetComponent<MeshRenderer>().enabled = false;
            point.GetComponent<SphereCollider>().enabled = false;
        }
    }

    public override void Interaction()
    {
        base.Interaction();
        GetIntoTheCar();
    }

    private void GetIntoTheCar()
    {
        ControlsManager.Instance.SwitchToCarControls();
        carHealthController.UpdateCarHealthUI();
        carController.ActivateCar(true);

        defaultPlayerScale = player.localScale.x;

        player.localScale = new Vector3(0.1f, 0.1f, 0.11f);
        player.transform.parent = transform;
        player.transform.localPosition = Vector3.up / 2;

        CameraManager.Instance.ChangeCameraTarget(transform, 40, 0.5f);

    }

    public void GetOutOfTheCar()
    {
        if (carController.CarActive == false)
            return;

        carController.ActivateCar(false);

        player.parent = null;
        player.position = GetExitPoint();
        player.transform.localScale = new Vector3(defaultPlayerScale, defaultPlayerScale, defaultPlayerScale);

        ControlsManager.Instance.SwitchToCharacterControls();
        PlayerAim aim = GameManager.Instance.Player.aim;

        CameraManager.Instance.ChangeCameraTarget(aim.GetAimCameraTarget(), 8.5f);
    }

    private Vector3 GetExitPoint()
    {
        for (int i = 0; i < exitPoints.Length; i++)
        {
            if (IsExitClear(exitPoints[i].position))
                return exitPoints[i].position;
        }

        return exitPoints[0].position;
    }

    private bool IsExitClear(Vector3 point)
    {
        Collider[] colliders = Physics.OverlapSphere(point, exitCheckRadius, ~whatToIngoreForExit);
        return colliders.Length == 0;
    }

    private void OnDrawGizmos()
    {
        if (exitPoints.Length > 0)
        {
            foreach (var point in exitPoints)
            {
                Gizmos.DrawWireSphere(point.position, exitCheckRadius);
            }
        }
    }
}
