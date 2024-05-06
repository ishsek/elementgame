using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] public HealthBarUIController HealthBarPrefab;
    [SerializeField] public Transform HealthBarLocation;
    [SerializeField] public Health HealthScript;
    [SerializeField] public Animator EnemyAnimator;
    [SerializeField] public Transform WaypointPrefab;

    public Rigidbody rb;
    protected EnemyAttack mAttack;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackRange;
    [SerializeField] protected float aggroRange;
    [SerializeField] protected int InitialDifficultyLevel;
    protected Transform target;
    [SerializeField] protected Vector3 playerDirection; // Can remove serialize after testing
    protected Vector3 playerDirectionNorm;
    protected Vector3 patrolDirection;
    public bool canMove = true;
    private GameObject playerReference;

    protected HealthBarUIController mHealthBar;
    protected Transform mWaypoint;

    protected bool mMoving = false;
    private enum State
    {
        Normal,
        Stunned,
        Rooted,
        Immobilized,
        Dead,
    }

    private State state;
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
        rb = GetComponent<Rigidbody>();
        mAttack = GetComponent<EnemyAttack>();
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
                Move();
                CheckAttack();
                break;

            case State.Stunned:
                HandleStun();
                break;

            case State.Rooted:
                LocatePlayer();
                RotateToPlayer();
                CheckAttack();
                break;

            case State.Immobilized:
                LocatePlayer();
                CheckAttack();
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

    protected virtual void Move()
    {
        // Check for location of player relative to enemy
        playerDirection = (target.position - transform.position);
        // Set y to zero to prevent vertical movement
        playerDirection.y = 0;
        if (playerDirection.magnitude <= aggroRange)
        {
            if (mMoving == false)
            {
                mMoving = true;
                if (EnemyAnimator != null)
                {
                    EnemyAnimator.SetTrigger(AnimationTriggersStatic.GetEnemyRunTrigger());
                }
            }
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirectionNorm), 0.15f));
            rb.velocity = playerDirectionNorm * speed;
        }
    }

    protected virtual void RotateToPlayer()
    {
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(playerDirectionNorm), 0.15f));
    }
    private void CheckAttack()
    {
        if (playerDirection.magnitude - attackRange < 0.01)
        {
            mMoving = false;
            rb.velocity = new Vector3(0, 0, 0);
            mAttack.OnAttack();
        }
    }

    private void HandleStun()
    {
        mStunTimer += Time.deltaTime;
        if (mStunTimer > mStunDuration)
        {
            mStunDuration = 0f;
            mStunTimer = 0f;
            state = State.Normal;
        }
    }

    public void SetStun(float duration)
    {
        mStunDuration += duration;
        state = State.Stunned;
    }

    public void Immobilize()
    {
        state = State.Immobilized;
    }
    public void Mobilize()
    {
        state = State.Normal;
    }

    public void Root()
    {
        state = State.Rooted;
    }

    public int GetInitialDifficultyLevel()
    {
        return InitialDifficultyLevel;
    }
}
