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
        waypointTarget = waypoint1;
    }
    protected override void Move()
    {
        base.Move();
        if (playerDirection.magnitude > aggroRange)
        {
            patrolDirection = (waypointTarget.position - transform.position);
            // Set y to zero to prevent vertical movement
            patrolDirection.y = 0;
            if (patrolDirection.magnitude > 0.1)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(patrolDirection), 0.15f);
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(waypointTarget.position.x, 0, waypointTarget.position.z), speed * Time.deltaTime);
            }
        }
    }
}