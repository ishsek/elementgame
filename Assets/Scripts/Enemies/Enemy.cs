using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] public Animator EnemyAnimator;

    Rigidbody rb;
    /*[SerializeField] protected float health;
    [SerializeField] protected float maxHealth;*/
    [SerializeField] protected float speed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float aggroRange;
    protected Transform target;
    [SerializeField] protected Vector3 playerDirection; // Can remove serialize after testing
    protected Vector3 patrolDirection;
    public bool canMove = true;

    protected bool mMoving = false;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (canMove)
        {
            Move();
        }
    }

    protected virtual void Move()
    {
        // Check for location of player relative to enemy
        playerDirection = (target.position - transform.position);
        // Set y to zero to prevent vertical movement
        playerDirection.y = 0;
        if (playerDirection.magnitude <= aggroRange)
        {
            if (playerDirection.magnitude - attackRange < 0.01)
            {
                mMoving = false;
                //Debug.Log("In range");
                EnemyAttack attack = GetComponent<EnemyAttack>();
                attack.OnAttack();
            }
            else
            {
                if (mMoving == false)
                {
                    mMoving = true;
                    if (EnemyAnimator != null)
                    {
                        EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
                    }
                }

                playerDirection = playerDirection.normalized;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirection), 0.15f);
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(target.position.x - playerDirection.x * attackRange, 0, target.position.z - playerDirection.z * attackRange), speed * Time.deltaTime);
            }
        }
    }
    public void Immobilize()
    {
        canMove = false;
    }
    public void Mobilize()
    {
        canMove = true;
    }
}
