using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public InputActionAsset actions;
    public Animator MyAnimator;
    public string MoveSpeedAnimationParameter;
    public Rigidbody rb;
    public InputAction aimAction;
    [SerializeField] private PlayerInput playerInput;

    [Header("Dodging")]
    public AnimationCurve DodgeCurve;
    [SerializeField] private float DodgeSpeed;
    [SerializeField] private float DodgeDuration;
    private float DodgeTime;

    [Header("Movement")]
    public float MaxSpeed;
    public AnimationCurve MovementCurve;
    // This tracks how long the player has been moving for.
    [SerializeField, ReadOnlyInspector] private float MoveTime;
    // The current player speed as a percentage of the MaxSpeed.
    [SerializeField, ReadOnlyInspector] private float CurrentSpeed;
    private Vector3 Movement;
    private Vector2 Move;

    [Header("Aiming")]
    public bool isGamepad;
    private Vector2 aimInput;
    public Vector3 aimDirection;

    //private IElement Shadow;

    private enum State
    {
        Normal,
        Dodging,
        Attacking,
    }
    // Maybe this should be public given there is a public function to change this variable
    private State state;

    private enum Element
    {
        Shadow,
        Light,
        Fire,
        Water,
        Air,
        Earth,
        None,
    }
    [Header("Element Switching")]
    [SerializeField] private Element ActiveElement;
    [SerializeField] private Element EquippedElement1;
    [SerializeField] private Element EquippedElement2;
    [SerializeField] private Element EquippedElement3;
    [SerializeField] private Element EquippedElement4;


    //**Consider consolidating move and aim inputs into a single function if merging attack and movement scripts**
    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        //playerInput = GetComponent<PlayerInput>();
        Movement = Vector3.zero;
        MoveTime = 0;
        state = State.Normal;
        aimAction = actions.FindActionMap("Player").FindAction("Aim");

        UpdateActiveElement(EquippedElement1);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMovementInput();
        switch (state)
        {
            case State.Normal:
                MovePlayer();
                break;
            case State.Dodging:
                HandleDodge();
                break;
            case State.Attacking:
                
                break;
        }
        HandleAim();
        UpdateAnimation();
    }

    private void UpdateMovementInput()
    {
        Movement.x = Move.x;
        Movement.z = Move.y;
    }

    private void MovePlayer()
    {
        if (Movement.magnitude > 0)
        {
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Movement), 0.15f));
            // Update MoveTime to reflect how long the player has been in motion.
            MoveTime += Time.deltaTime;
            CurrentSpeed = MovementCurve.Evaluate(MoveTime);
            rb.velocity = Movement * MaxSpeed * CurrentSpeed;
        }
        else
        {
            // Reset movetime to zero when player stops moving.
            HaltMovement();
        }
    }

    public void OnDodge()
    {
        if ( state == State.Normal)
        {
            state = State.Dodging;
            DodgeTime = 0f;
        }
    }

    public void HaltMovement()
    {
        MoveTime = 0;
        rb.velocity = new Vector3(0, 0, 0);
    }

    private void HandleDodge()
    {
        DodgeTime += Time.deltaTime;
        Movement = Movement.normalized;
        rb.velocity = Movement * DodgeSpeed * DodgeCurve.Evaluate(DodgeTime / DodgeDuration);
        if (DodgeTime > DodgeDuration)
        {
            state = State.Normal;
        }
    }

    private void UpdateAnimation()
    {
        // Should refactor speed to be current speed and have it smooth into top speed rather than instantly top speed.
        // This will allow for the animator to blend the animation between standing still and running more naturally.
        MyAnimator.SetFloat(MoveSpeedAnimationParameter, Movement.magnitude * CurrentSpeed);
    }

    void OnEnable()
    {
        actions.FindActionMap("Player").Enable();
    }
    void OnDisable()
    {
        actions.FindActionMap("Player").Disable();
    }

    private void HandleAim()
    {
        // Get input value for aim
        aimInput = aimAction.ReadValue<Vector2>();

        // Handle gamepad control
        if (isGamepad)
        {
            aimDirection = Vector3.right * aimInput.x + Vector3.forward * aimInput.y;
        }
        // Handle mouse control
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(aimInput);
            Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
            float rayDistance;

            if (groundPlane.Raycast(ray, out rayDistance))
            {
                aimDirection = ray.GetPoint(rayDistance);
            }
        }
    }

    public void rotateToAim()
    {
        // Rotate player to aim direction
        if (isGamepad)
        {
            if (aimDirection.sqrMagnitude > 0.0f)
            {
                rb.rotation = Quaternion.LookRotation(aimDirection, Vector3.up);
            }
        }
        else
        {
            LookAt(aimDirection);
        }
    }

    private void LookAt(Vector3 lookPoint)
    {
        Vector3 DirectionVector = lookPoint - transform.position;
        DirectionVector.y = 0;
        rb.rotation = Quaternion.LookRotation(DirectionVector, Vector3.up);
    }

    public void SwapToElement1(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UpdateActiveElement(EquippedElement1);
        }
    }

    public void SwapToElement2(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UpdateActiveElement(EquippedElement2);
        }
    }

    public void SwapToElement3(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UpdateActiveElement(EquippedElement3);
        }
    }

    public void SwapToElement4(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            UpdateActiveElement(EquippedElement4);
        }
    }

    private void UpdateActiveElement(Element NewElement)
    {
        if (ActiveElement != NewElement)
        {
            DisablePreviousElement(ActiveElement);
            switch (NewElement)
            {
                case Element.Shadow:
                    SetElementShadow();
                    break;
                case Element.Light:

                    break;
                case Element.Fire:

                    break;
                case Element.Water:

                    break;
                case Element.Air:

                    break;
                case Element.Earth:

                    break;
                case Element.None:

                    break;
            }
        }
    }
    private void DisablePreviousElement(Element OldElement)
    {
        switch (OldElement)
        {
            case Element.Shadow:
                playerInput.actions.FindActionMap("Shadow").Disable();
                break;
            case Element.Light:

                break;
            case Element.Fire:

                break;
            case Element.Water:

                break;
            case Element.Air:

                break;
            case Element.Earth:

                break;
            case Element.None:

                break;
        }
    }

    private void SetElementShadow()
    {
        playerInput.actions.FindActionMap("Shadow").Enable();
        ActiveElement = Element.Shadow;
    }

    public void SetStateAttacking()
    {
        state = State.Attacking;
    }

    public void SetStateNormal()
    {
        state = State.Normal;
        //MyAnimator.SetTrigger("Blend Tree");
    }
    public void SetStateDodging()
    {
        state = State.Dodging;
    }

    public void OnDeviceChange(PlayerInput input)
    {
        isGamepad = input.currentControlScheme.Equals("Gamepad") ? true : false;
    }

}
