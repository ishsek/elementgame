using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.Rendering.DebugUI;
using static Weapon;

public class BlackHole : MonoBehaviour
{
    public float VoidDamage;
    [SerializeField] private float m_VoidPullStrength;
    [SerializeField] private float m_VoidTickRate;
    [SerializeField] private float m_VoidRadius;
    [SerializeField] private float m_VoidDuration;
    private float mEndTime;
    // Table that records when the next damage tick should occur for each enemy present.
    private Dictionary<Collider, float> mTargetTable = new Dictionary<Collider, float>();

    void Start()
    {
        mEndTime = Time.time + m_VoidDuration;
    }

    void Update()
    {
        if (Time.time >= mEndTime)
        {
            foreach (KeyValuePair<Collider, float> target in mTargetTable)
            {
                if (target.Key != null)
                {
                    if (target.Key.TryGetComponent<Enemy>(out Enemy Enemy))
                    {
                        Enemy.SetStateNormal();
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy Enemy))
        {
            Enemy.SetStateNormal();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") && !mTargetTable.ContainsKey(other))
        {
            mTargetTable[other] = float.NegativeInfinity;
            other.GetComponent<Enemy>().Root();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        float timer;
        if (!mTargetTable.TryGetValue(other, out timer)) return; //if not in table, it's not an enemy

        if (Time.time > timer)
        {
            mTargetTable[other] = Time.time + m_VoidTickRate;
            // Damage the enemy
            if (other.TryGetComponent<Health>(out Health EnemyHP))
            {
                EnemyHP.TakeDamage(VoidDamage);
            }
            // Pull the enemy
            if (other.TryGetComponent<Rigidbody>(out Rigidbody rb))
            {
                Vector3 PullDirection = transform.position - other.transform.position;
                PullDirection.y = 0;
                // ** Uncomment and divide PullDirection by PullDistance to have pull strength inversely proportional to distance from the centre of the void. **
                //PullDirection = PullDirection.normalized;
                //float PullDistance = Vector3.Distance(other.transform.position, transform.position);
                rb.velocity = PullDirection * m_VoidPullStrength;
            }
        }
    }
}
