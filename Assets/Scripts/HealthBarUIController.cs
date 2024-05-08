using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarUIController : MonoBehaviour
{
    [SerializeField] private RectTransform m_MyTransform;
    [SerializeField] private Transform m_FollowTarget;
    [SerializeField] private Image m_HealthBarImage;
    [SerializeField] private Image m_DecayingHealthBarImage;

    // Update is called once per frame
    void Update()
    {
        if (m_FollowTarget == null)
        {
            return;
        }

        m_MyTransform.position = m_FollowTarget.position;

        UpdateRotationWithCamera();
    }

    private void UpdateRotationWithCamera()
    {
        Quaternion newRotation = GameManagerStaticHelper.GetMainCamera().transform.rotation;
        newRotation = Quaternion.Euler(newRotation.eulerAngles.x - newRotation.eulerAngles.x * 2, newRotation.eulerAngles.y - 180, newRotation.eulerAngles.z);
        m_MyTransform.rotation = newRotation;
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

    public Image GetDecayingHealthBarImage()
    {
        return m_DecayingHealthBarImage;
    }
}
