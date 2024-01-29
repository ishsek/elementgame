using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Animator MyAnimator;
    public string MoveSpeedAnimationParameter;

    [Header("Tuning")]
    public float MaxSpeed;
    public float DodgeSpeed;
    public AnimationCurve MovementCurve;
    public bool canMove;

    [Header("Read Only")]
    // This tracks how long the player has been moving for.
    [SerializeField, ReadOnlyInspector] private float MoveTime;
    // The current player speed as a percentage of the MaxSpeed.
    [SerializeField, ReadOnlyInspector] private float CurrentSpeed;

    private enum State
    {
        Normal,
        Dodging,
    }
    private State state;

    private Vector3 Movement;
    private Vector2 Move;

    //**Consider consolidating move and aim inputs into a single function if merging attack and movement scripts**
    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Awake()
    {
        Movement = Vector3.zero;
        MoveTime = 0;
        canMove = true;
        state = State.Normal;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.Normal:
                MovePlayer();
                UpdateAnimation();
                break;
            case State.Dodging:
                HandleDodge();
                break;
        }
    MovePlayer();
    UpdateAnimation();
    }

    private void MovePlayer()
    {
        Movement.x = Move.x;
        Movement.z = Move.y;

        if (canMove && Movement.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Movement), 0.15f);
            // Update MoveTime to reflect how long the player has been in motion.
            MoveTime += Time.deltaTime;
            CurrentSpeed = MovementCurve.Evaluate(MoveTime);
            transform.Translate(Movement * CurrentSpeed * MaxSpeed * Time.deltaTime, Space.World);
        }
        else
        {
            // Reset movetime to zero when player stops moving.
            MoveTime = 0;
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        state = State.Dodging;
        DodgeSpeed = 50f;
    }

    private void HandleDodge()
    {
        transform.position += Movement * DodgeSpeed * Time.deltaTime;
        DodgeSpeed -= DodgeSpeed * 10f * Time.deltaTime;
        if (DodgeSpeed < 5f)
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


}
