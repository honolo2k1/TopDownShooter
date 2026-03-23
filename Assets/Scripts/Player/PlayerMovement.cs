using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Player player;

    private PlayerControls controls;
    private CharacterController characterController;
    private Animator animator;

    [Header("Movement Info")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float turnSpeed;
    private float verticalVelocity;
    private float speed;

    public Vector2 moveInput { get; private set; }
    private Vector3 movementDirection;

    private float gravityScale = 9.81f;
    private bool isRunning;

    private AudioSource walkSFX;
    private AudioSource runSFX;
    private bool canPlayFootsteps;

    private void Start()
    {
        player = GetComponent<Player>();

        walkSFX = player.Sound.WalkSFX;
        runSFX = player.Sound.RunSFX;
        Invoke(nameof(AllowfootstepsSFX), 1f);

        characterController = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();

        speed = walkSpeed;

        AssignInputEvents();
    }

    private void Update()
    {
        if (player.Health.IsDead)
        {
            return;
        }
        ApplyMovement();
        ApplyRotaion();
        AnimatorControllers();
    }

    private void AssignInputEvents()
    {
        controls = player.Controls;

        controls.Character.Movement.performed += context => moveInput = context.ReadValue<Vector2>();
        controls.Character.Movement.canceled += context =>
        {
            StopFootstepsSFX();
            moveInput = Vector2.zero;
        };


        controls.Character.Run.performed += context =>
        {
            speed = runSpeed;
            isRunning = true;
        };
        controls.Character.Run.canceled += context =>
        {
            speed = walkSpeed;
            isRunning = false;
        };
    }

    private void AnimatorControllers()
    {
        float xVelocity = Vector3.Dot(movementDirection.normalized, transform.right);
        float zVelocity = Vector3.Dot(movementDirection.normalized, transform.forward);

        animator.SetFloat("xVelocity", xVelocity, 0.1f, Time.deltaTime);
        animator.SetFloat("zVelocity", zVelocity, 0.1f, Time.deltaTime);

        bool playRunAnim = isRunning && movementDirection.magnitude > 0;
        animator.SetBool("isRunning", playRunAnim);
    }

    private void ApplyRotaion()
    {
        Vector3 lookingDirection = player.Aim.GetMouseHitInfo().point - transform.position;
        lookingDirection.y = 0f;
        lookingDirection.Normalize();

        //transform.forward = lookingDirection;

        Quaternion desiredRotation = Quaternion.LookRotation(lookingDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRotation, turnSpeed * Time.deltaTime);
    }

    private void ApplyMovement()
    {
        movementDirection = new Vector3(moveInput.x, 0, moveInput.y);
        ApplyGravity();
        if (movementDirection.magnitude > 0)
        {
            PlayFootstepsSFX();
            characterController.Move(movementDirection * Time.deltaTime * speed);
        }
    }

    private void PlayFootstepsSFX()
    {
        if (canPlayFootsteps == false)
            return;

        if (isRunning)
        {
            if (runSFX.isPlaying == false)
                runSFX.Play();
        }
        else
        {
            if (walkSFX.isPlaying == false)
                walkSFX.Play();
        }
    }
    private void StopFootstepsSFX()
    {
        walkSFX.Stop();
        runSFX.Stop();
    }
    private void AllowfootstepsSFX() => canPlayFootsteps = true;


    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            verticalVelocity -= gravityScale * Time.deltaTime;
            movementDirection.y = verticalVelocity;
        }
        else
        {
            verticalVelocity = -0.5f;
        }
    }


}
