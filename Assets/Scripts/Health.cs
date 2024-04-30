using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class Health : MonoBehaviour
{
    [SerializeField] private Image HealthBar;

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

            HealthBar.fillAmount = mCurrentHealthOnUI / maxHealth;
        }
    }

    public virtual void TakeDamage(float damage)
    {
        HealthBar.fillAmount = health / maxHealth;
        mCurrentHealthOnUI = health;

        health -= damage;

        if (HealthBar != null)
        {
            mHealthDecaying = true;
            mHealthDecayPerSecond = (mCurrentHealthOnUI - health) / m_HealthDecayDuration;
        }

        if (health <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void SetHealthBar(Image healthBarRef)
    {
        HealthBar = healthBarRef;
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
