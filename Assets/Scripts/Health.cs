using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Health : MonoBehaviour
{
    [SerializeField] private Image HealthBar;
    [SerializeField] private Image m_BackgroundHealthBar;
    [SerializeField] private HealthBarUIController m_HealthBarController;

    [SerializeField] private bool m_IsEnemy = false;
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] private float m_DespawnAfterDeathDelay = 10;

    private float m_HealthDecayDuration = 0.2f;
    private float mCurrentHealthOnUI;
    private float mHealthDecayPerSecond;
    private float mTimeDead = 0;
    private bool mHealthDecaying = false;
    private bool mIsDead = false;
    private bool mDespawnAnimationComplete = false;

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
        mCurrentHealthOnUI = health;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();
        UpdateDeath();
    }

    private void UpdateHealthBar()
    {
        if (mHealthDecaying == true)
        {
            mCurrentHealthOnUI -= Time.deltaTime * mHealthDecayPerSecond;

            if (mCurrentHealthOnUI <= health)
            {
                mCurrentHealthOnUI = health;
                mHealthDecaying = false;
            }

            m_BackgroundHealthBar.fillAmount = mCurrentHealthOnUI / maxHealth;
        }
    }

    private void UpdateDeath()
    {
        if ((mIsDead == true) && (mDespawnAnimationComplete == false))
        {
            mTimeDead += Time.deltaTime;

            if (mTimeDead >= m_DespawnAfterDeathDelay)
            {
                //probably also destroy the game object here, after doing a despawn animation

                mDespawnAnimationComplete = true;

                if (m_IsEnemy == true)
                {
                    Destroy(m_HealthBarController.gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        m_BackgroundHealthBar.fillAmount = health / maxHealth;
        mCurrentHealthOnUI = health;

        health -= damage;

        HealthBar.fillAmount = health / maxHealth;
        mHealthDecaying = true;
        mHealthDecayPerSecond = (mCurrentHealthOnUI - health) / m_HealthDecayDuration;

        if (health <= 0)
        {
            // because this script is being used for enemies and players and we don't want
            // to destroy the gameobjects, each player or enemy script should be checking the health
            // value on this script to determine what to do when the enemy is dead.
            //Destroy(gameObject);

            mIsDead = true;
        }
    }

    public void Heal(float amount)
    {

        if (health + amount > maxHealth)
        {
            health = maxHealth;
        }
        else
        {
            health += amount;
        }

        HealthBar.fillAmount = health / maxHealth;

    }

    public void SetHealthBar(Image healthBarRef, Image backgorundHealthBarRef, HealthBarUIController healthBarController)
    {
        HealthBar = healthBarRef;
        m_BackgroundHealthBar = backgorundHealthBarRef;
        m_HealthBarController = healthBarController;
    }

    public float GetHealth()
    {
        return health;
    }

    public float GetHealthPercent()
    {
        return health / maxHealth;
    }
}
