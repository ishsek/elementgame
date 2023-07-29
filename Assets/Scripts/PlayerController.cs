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
    public AnimationCurve MovementCurve;

    [Header("Read Only")]
    // This tracks how long the player has been moving for. Left public for testing purposes. Switch to private later.
    [SerializeField, ReadOnlyInspector] private float MoveTime;
    // The current player speed as a percentage of the MaxSpeed.
    [SerializeField, ReadOnlyInspector] private float CurrentSpeed;

    private Vector3 Movement;
    private Vector2 Move;

    public void OnMove(InputAction.CallbackContext context)
    {
        Move = context.ReadValue<Vector2>();
    }

    // Start is called before the first frame update
    void Start()
    {
        Movement = Vector3.zero;
        MoveTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        UpdateAnimation();
    }

    private void MovePlayer()
    {
        Movement.x = Move.x;
        Movement.z = Move.y;

        if (Movement.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Movement), 0.15f);
            // Update MoveTime to reflect how long the player has been in motion.
            MoveTime += Time.deltaTime;
            CurrentSpeed = MovementCurve.Evaluate(MoveTime);
        }
        else
        {
            // Reset movetime to zero when player stops moving.
            MoveTime = 0;
        }

        transform.Translate(Movement * CurrentSpeed * MaxSpeed * Time.deltaTime, Space.World);
    }

    private void UpdateAnimation()
    {
        // Should refactor speed to be current speed and have it smooth into top speed rather than instantly top speed.
        // This will allow for the animator to blend the animation between standing still and running more naturally.
        MyAnimator.SetFloat(MoveSpeedAnimationParameter, Movement.magnitude * CurrentSpeed);
    }
}
