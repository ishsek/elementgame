using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : Enemy
{
    private Transform waypointTarget;
    public Transform waypoint1; // alternate target between multiple waypoints to create patrolling enemies 
    protected override void Awake()
    {
        base.Awake();
        waypointTarget = waypoint1 = mWaypoint;
    }
    protected override void Move()
    {
        base.Move();
        if (playerDirection.magnitude > aggroRange)
        {
            if (waypointTarget == null)
            {
                return;
            }

            patrolDirection = (waypointTarget.position - transform.position);

            // Set y to zero to prevent vertical movement
            patrolDirection.y = 0;
            if (patrolDirection.magnitude > 0.1)
            {
                patrolDirection = patrolDirection.normalized;
                rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(patrolDirection), 0.15f));
                rb.velocity = patrolDirection * speed;

                if (mMoving == false)
                {
                    mMoving = true;
                    EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
                }
            }
            else
            {
                if (mMoving == true)
                {
                    mMoving = false;
                    rb.velocity = new Vector3 (0, 0, 0);
                    EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyIdleTrigger());
                }
            }
        }
    }
}