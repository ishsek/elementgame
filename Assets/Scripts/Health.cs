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
    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        HealthBar.fillAmount = health / maxHealth;

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }
}
