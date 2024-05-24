using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public enum State
    {
        Normal,
        Agro,
        Leash,
        Attacking,
        Targeting,
        Stunned,
        Rooted,
        Immobilized,
        Dead,
    }

    [Header("References")]
    [SerializeField] public HealthBarUIController HealthBarPrefab;
    [SerializeField] public Transform HealthBarLocation;
    [SerializeField] public Health HealthScript;
    [SerializeField] public Animator EnemyAnimator;
    [SerializeField] public Transform WaypointPrefab;
    private GameObject playerReference;
    protected HealthBarUIController mHealthBar;
    protected Transform mWaypoint;

    public Rigidbody rb;

    [Header("General Attack Settings")]
    [SerializeField] private float m_AttackCD = 1f; //seconds
    protected float mLastAttack = -9999f;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float aggroRange;
    [SerializeField] protected int InitialDifficultyLevel;
    protected Transform target;
    [SerializeField] protected Vector3 playerDirection; // Can remove serialize after testing
    protected Vector3 playerDirectionNorm;
    protected Vector3 patrolDirection;

    protected bool mMoving = false;

    protected State state;
    private float mStunTimer = 0f;
    private float mStunDuration = 0f;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        playerReference = GameObject.FindGameObjectWithTag("Player");

        if (playerReference != null)
        {
            target = playerReference.GetComponent<Transform>();
        }
    }

    protected virtual void Awake()
    {
        //rb = GetComponent<Rigidbody>();
        //mAttack = GetComponent<EnemyAttack>();
        state = State.Normal;

        mWaypoint = Instantiate(WaypointPrefab, transform.position, transform.rotation);

        mHealthBar = Instantiate(HealthBarPrefab);
        mHealthBar.SetTarget(HealthBarLocation);

        if (HealthScript != null)
        {
            HealthScript.SetHealthBar(mHealthBar.GetHealthBarImage(), mHealthBar.GetDecayingHealthBarImage());
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if ((state != State.Dead) && (HealthScript.GetHealth() <= 0))
        {
            state = State.Dead;
            // do a ragdoll or something
            EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyDeathTrigger());
        }

        switch (state)
        {
            case State.Normal:
                LocatePlayer();
                ChooseMovementOption();
                break;

            case State.Leash:
                LocatePlayer();
                ChooseMovementOption();
                Leashing();
                break;

            case State.Agro:
                LocatePlayer();
                ChooseMovementOption();
                Agroing();
                break;

            case State.Attacking:
                DoAttack();
                break;

            case State.Targeting:
                LocatePlayer();
                RotateToPlayer();
                CheckTargetInRange();
                break;

            case State.Stunned:
                HandleStun();
                break;

            case State.Rooted:
                LocatePlayer();
                RotateToPlayer();
                //CheckTargetInRange();
                break;

            case State.Immobilized:
                LocatePlayer();
                break;

            case State.Dead:
                break;

            default:
                break;
        }
    }
    protected virtual void LocatePlayer()
    {
        if (playerReference == null)
        {
            return;
        }

        // Check for location of player relative to enemy
        playerDirection = (target.position - transform.position);
        // Set y to zero to prevent vertical movement
        playerDirection.y = 0;
        playerDirectionNorm = playerDirection.normalized;
    }

    protected virtual void ChooseMovementOption()
    {
        if (CheckAttackRange() && CheckAttackCD())
        {
            SetStateAttacking();
            return;
        }
        else if (CheckAgro())
        {
            SetStateAgro();
            return;
        }
        else if (CheckLeash())
        {
            SetStateLeash();
            return;
        }
        SetStateNormal();
    }

    private void CheckTargetInRange()
    {
        if (CheckAttackRange())
        {
            if (CheckAttackCD())
            {
                SetStateAttacking();
                return;
            }
            return;
        }
        SetStateNormal();
    }

    private bool CheckAttackRange()
    {
        if (playerDirection.magnitude - attackRange < 0.01)
        {
            return true;
        }
        return false;
    }

    private bool CheckAttackCD()
    {
        if (Time.time > mLastAttack + m_AttackCD)
        {
            return true;
        }
        return false;
    }

    protected bool CheckAgro()
    {
        if (playerDirection.magnitude <= aggroRange)
        {
            return true;
        }
        return false;
    }

    protected bool CheckLeash()
    {
        patrolDirection = (mWaypoint.position - transform.position);
        patrolDirection.y = 0;
        if (patrolDirection.magnitude > 1)
        {
            return true;
        }
        return false;
    }

    protected virtual void RotateToPlayer()
    {
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirectionNorm), 0.15f));
    }

    private void HandleStun()
    {
        mStunTimer += Time.deltaTime;
        if (mStunTimer > mStunDuration)
        {
            mStunDuration = 0f;
            mStunTimer = 0f;
            //EnemyAnimator.ResetTrigger(AnimationTriggersStatic.GetEnemyIdleTrigger());
            state = State.Normal;
        }
    }

    protected virtual void DoAttack()
    {
        return;
    }

    protected virtual void Leashing()
    {
        patrolDirection = patrolDirection.normalized;
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(patrolDirection), 0.15f));
        rb.velocity = patrolDirection * speed;
    }

    protected virtual void Agroing()
    {
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirectionNorm), 0.15f));
        rb.velocity = playerDirectionNorm * speed;
    }

    public virtual void SetStun(float duration)
    {
        if (state != State.Dead)
        {
            mStunDuration += duration;
            state = State.Stunned;
            // add idle/stagger animation call in subclass
        }
    }

    public void Immobilize()
    {
        state = State.Immobilized;
    }

    public void Root()
    {
        state = State.Rooted;
    }

    public virtual void SetStateAttacking()
    {
        state = State.Attacking;
        rb.velocity = new Vector3(0, 0, 0);
        mLastAttack = Time.time;
        // Add attack animation call and actual attack in subclass override
    }

    public virtual void SetStateAgro()
    {
        if (state != State.Agro)
        {
            state = State.Agro;
        }
        // Add move animation call in subclass override
    }

    public virtual void SetStateLeash()
    {
        if (state != State.Leash)
        {
            state = State.Leash;
        }
        // Add move animation call in subclass override
    }

    public virtual void SetStateNormal()
    {
        if (state != State.Normal)
        {
            state = State.Normal;
        }
        // Add idle animation call in subclass override
    }

    public virtual void SetStateTargeting()
    {
        state = State.Targeting;
        // Add idle animation call in subclass override
    }

    public int GetInitialDifficultyLevel()
    {
        return InitialDifficultyLevel;
    }

    public State GetState()
    {
        return state;
    }
}