using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    private BaseStateMachine<PlayerScript> playerStateMachine;

    private Animator animator;
    private CharacterController characterController;
    private AnimationController animationController;

    private PlayerInput playerInput;
    public StarterAssetsInputs input;
    private GameObject _mainCamera;

    [Header("Movement")]
    public float walkSpeed = 2.0f;
    public float sprintSpeed = 5.335f;
    [Range(0.0f, 0.3f)] public float rotationSmoothness = 0.12f;
    public float movementAcceleration = 10.0f;

    private Vector3 targetDirection;
    private float moveSpeed;
    private float _animationBlend;
    private float targetRotation = 0.0f;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    [Header("Grounded Vars")]
    public bool grounded = true;
    public float groundedOffset = -0.14f;
    [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
    public float groundedRadius = 0.28f;
    [Tooltip("What layers the character uses as ground")]
    public LayerMask groundLayers;

    [Space(10)]

    [Header("Jump Vars")]
    public float JumpHeight = 1.2f;
    public float jumpCooldown;
    public Action onLanded;
    private float jumpTimeoutDelta;

    [Space(10)]

    [Header("Gravity")]
    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    public float fallTimeout = 0.15f;
    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    public float gravity = -15.0f;
    private float fallTimeoutDelta;
    public Action onStartFall;

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    public Vector3 GravityForce
    {
        get { return new Vector3(0.0f, _verticalVelocity, 0.0f); }
    }

    public float TargetRotation
    {
        get { return targetRotation; }
        set { targetRotation = value; }
    }

    private void Start()
    {
        if (_mainCamera == null)
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        animator = GetComponent<Animator>();
        input = GetComponent<StarterAssetsInputs>();
        playerInput = GetComponent<PlayerInput>();
        characterController = GetComponent<CharacterController>();

        AssignAnimationIDs();
        SetupStates();
    }

    private void SetupStates()
    {
        playerStateMachine = new BaseStateMachine<PlayerScript>(this, new IdleState());
        playerStateMachine.AddState(new MoveState());
        playerStateMachine.AddState(new AimState());
        playerStateMachine.AddState(new ThrowState());
        playerStateMachine.AddState(new JumpState());
        playerStateMachine.AddState(new FallState());
        playerStateMachine.AddState(new LandState());
        playerStateMachine.AddState(new DashState());

    }

    private void Update()
    {
        playerStateMachine.Update(Time.deltaTime);

        UpdateJumpStatus();
        GroundedCheck();
    }

    public Vector3 GetMoveDirection()
    {
        float targetSpeed = input.sprint ? sprintSpeed : walkSpeed;

        if (input.move == Vector2.zero)
            targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(characterController.velocity.x, 0.0f, characterController.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = input.analogMovement ? input.move.magnitude : 1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            //_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
            moveSpeed = targetSpeed * inputMagnitude;

            // round speed to 3 decimal places
            moveSpeed = Mathf.Round(moveSpeed * 1000f) / 1000f;
        }
        else
            moveSpeed = targetSpeed;

        //_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
        _animationBlend = targetSpeed;

        if (_animationBlend < 0.01f)
            _animationBlend = 0f;

        Vector3 inputDirection = new Vector3(input.move.x, 0.0f, input.move.y).normalized;

        if (input.move != Vector2.zero)
        {
            targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;

            DebugExtension.DebugArrow(transform.position, inputDirection, Color.yellow, 0.3f, Time.deltaTime);
            /*
            float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                RotationSmoothTime);

            // rotate to face input direction relative to camera position
            transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            */
        }

        targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        animator.SetFloat(_animIDSpeed, _animationBlend);
        animator.SetFloat(_animIDMotionSpeed, inputMagnitude);

        return targetDirection;
    }

    public void UpdateCharacterRotation(float fTargetRotation)
    {
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, fTargetRotation, ref _rotationVelocity,
                rotationSmoothness);

        // rotate to face input direction relative to camera position
        transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
    }

    public void ApplyMovement(Vector3 pDirection, Vector3 pGravityForce)
    {
        characterController.Move(pDirection * (moveSpeed * Time.deltaTime) + pGravityForce * Time.deltaTime);
    }

    public void CalculateVerticalVelocity()
    {
        if (grounded)
        {
            fallTimeoutDelta = fallTimeout;

            if (_verticalVelocity < 0.0f)
                _verticalVelocity = -2f;
        }
        else
        {
            if (fallTimeoutDelta >= 0.0f)
                fallTimeoutDelta -= Time.deltaTime;
        }

        if (_verticalVelocity < _terminalVelocity)
            _verticalVelocity += gravity * Time.deltaTime;
    }

    public void UpdateJumpStatus()
    {
        if (grounded)
        {
            // jump timeout
            if (jumpTimeoutDelta >= 0.0f)
            {
                jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            jumpTimeoutDelta = jumpCooldown;
            input.jump = false;
        }
    }

    public void Jump()
    {
        // the square root of H * -2 * G = how much velocity needed to reach desired height
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * gravity);

        animator.SetBool(_animIDJump, true);
    }

    private void GroundedCheck()
    {
        // set sphere position, with offset
        Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - groundedOffset,
            transform.position.z);

        bool wasGrounded = grounded;

        grounded = Physics.CheckSphere(spherePosition, groundedRadius, groundLayers,
            QueryTriggerInteraction.Ignore);

        if (!wasGrounded && grounded)
            OnLanded();
        else if (wasGrounded && !grounded)
            OnStartFalling();

         animator.SetBool(_animIDGrounded, grounded);
    }

    public void OnDashInput()
    {

    }
      
    public void OnStartFalling()
    {
        onStartFall?.Invoke();
    }

    public void OnLanded()
    {
        animator.SetBool(_animIDJump, false);
        animator.SetBool(_animIDFreeFall, false);

        onLanded?.Invoke();
    }

    public Vector2 GetMovementInput()
    {
        return input.move;
    }

    public float GetCameraDirection()
    {
        return Mathf.Atan2(Vector3.forward.x, Vector3.forward.z) * Mathf.Rad2Deg +
                              _mainCamera.transform.eulerAngles.y;
    }

    public AnimationController GetAnimationController()
    {
        if (animationController == null)
            animationController = GetComponent<AnimationController>();

        return animationController;
    }

    public bool CanJump()
    {
        if (grounded && jumpTimeoutDelta <= 0.0f)
            return true;
        else
            return false;
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    private void OnGUI()
    {
        GUILayout.Box(playerStateMachine.CurrentStateType.ToString());
        playerStateMachine.OnGUI();
    }
}
