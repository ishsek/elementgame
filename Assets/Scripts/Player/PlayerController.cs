using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Animator MyAnimator;
    public string MoveSpeedAnimationParameter;
    private Rigidbody rb;

    [Header("Tuning")]
    public float MaxSpeed;
    [SerializeField] private float DodgeSpeed;
    [SerializeField] private float DodgeDuration;
    public AnimationCurve MovementCurve;
    public AnimationCurve DodgeCurve;
    public bool canMove;

    [Header("Read Only")]
    // This tracks how long the player has been moving for.
    [SerializeField, ReadOnlyInspector] private float MoveTime;
    // The current player speed as a percentage of the MaxSpeed.
    [SerializeField, ReadOnlyInspector] private float CurrentSpeed;
    private float DodgeTime;

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
        rb = GetComponent<Rigidbody>();
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
                break;
            case State.Dodging:
                HandleDodge();
                break;
        }
    UpdateAnimation();
    }

    private void MovePlayer()
    {
        Movement.x = Move.x;
        Movement.z = Move.y;

        if (canMove && Movement.magnitude > 0)
        {
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Movement), 0.15f);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Movement), 0.15f));
            // Update MoveTime to reflect how long the player has been in motion.
            MoveTime += Time.deltaTime;
            CurrentSpeed = MovementCurve.Evaluate(MoveTime);
            //transform.Translate(Movement * CurrentSpeed * MaxSpeed * Time.deltaTime, Space.World);
            rb.velocity = Movement * MaxSpeed * CurrentSpeed;
        }
        else
        {
            // Reset movetime to zero when player stops moving.
            MoveTime = 0;
            rb.velocity = new Vector3(0, 0, 0);
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            state = State.Dodging;
            DodgeTime = 0f;
            canMove = false;
            //rb.AddForce(Movement * 50f, ForceMode.Impulse);
        }
    }

    private void HandleDodge()
    {
        DodgeTime += Time.deltaTime;
        Movement = Movement.normalized;
        rb.velocity = Movement * DodgeSpeed * DodgeCurve.Evaluate(DodgeTime / DodgeDuration);
        if (DodgeTime > DodgeDuration)
        {
            state = State.Normal;
            canMove = true;
        }
    }

    private void UpdateAnimation()
    {
        // Should refactor speed to be current speed and have it smooth into top speed rather than instantly top speed.
        // This will allow for the animator to blend the animation between standing still and running more naturally.
        MyAnimator.SetFloat(MoveSpeedAnimationParameter, Movement.magnitude * CurrentSpeed);
    }


}
