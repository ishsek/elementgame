using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUIController : MonoBehaviour
{
    [SerializeField] private RectTransform m_MyTransform;
    [SerializeField] private Transform m_FollowTarget;
    [SerializeField] private Image m_HealthBarImage;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_FollowTarget == null)
        {
            return;
        }

        m_MyTransform.position = m_FollowTarget.position;
    }

    public void SetTarget(Transform target)
    {
        m_FollowTarget = target;
    }

    public void SetHealthUI(float healthPercent)
    {
        m_HealthBarImage.fillAmount = healthPercent;
    }

    public Image GetHealthBarImage()
    {
        return m_HealthBarImage;
    }
}
