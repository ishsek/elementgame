using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Health : MonoBehaviour
{
    [SerializeField] private Image HealthBar;
    [SerializeField] private Image m_BackgroundHealthBar;

    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;

    private float m_HealthDecayDuration = 0.2f;
    private float mCurrentHealthOnUI;
    private float mHealthDecayPerSecond;
    private bool mHealthDecaying = false;

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
        }
    }

    public void SetHealthBar(Image healthBarRef, Image backgorundHealthBarRef)
    {
        HealthBar = healthBarRef;
        m_BackgroundHealthBar = backgorundHealthBarRef;
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
