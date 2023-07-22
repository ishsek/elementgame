using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Animator MyAnimator;
    public string MoveSpeedAnimationParameter;
    public float Speed;

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
        }

        transform.Translate(Movement * Speed * Time.deltaTime, Space.World);
    }

    private void UpdateAnimation()
    {
        // Should refactor speed to be current speed and have it smooth into top speed rather than instantly top speed.
        // This will allow for the animator to blend the animation between standing still and running more naturally.
        MyAnimator.SetFloat(MoveSpeedAnimationParameter, Movement.magnitude);
    }
}
