using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Controls UI buttons for class based skills
public class SkillButtonController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image m_ButtonImage;
    [SerializeField] private TextMeshProUGUI m_CooldownText;

    [Header("Design")]
    [SerializeField] private Color ActiveButtonColor;
    [SerializeField] private Color CooldownButtonColor;

    private string mCooldownFormat = "f1";
    private float mCurrentCooldown;
    private bool mButtonValid = true;

    public enum SkillButtonStates
    {
        Available,
        Disabled,
        Cooldown,
    }

    private SkillButtonStates mButtonState;

    private void Awake()
    {
        if ((m_ButtonImage == null) || (m_CooldownText == null))
        {
            Debug.LogWarning("Missing reference in SkillButtonController on: " + gameObject);
            mButtonValid = false;
        }
    }

    private void Update()
    {
        if (mButtonValid == false)
        {
            return;
        }

        UpdateButtonState();
    }

    // Internal set button state to be used within this class
    private void SetButtonState(SkillButtonStates newButtonState, float newCooldownTime = 0)
    {
        if (mButtonValid == false)
        {
            Debug.LogWarning("Attempting to change state in SkillButtonController on invalid button on: " + gameObject);
            return;
        }

        switch (newButtonState)
        {
            case SkillButtonStates.Available:
                m_ButtonImage.color = ActiveButtonColor;
                mCurrentCooldown = 0;
                m_CooldownText.SetText("");
                break;
            case SkillButtonStates.Cooldown:
                m_ButtonImage.color = CooldownButtonColor;
                mCurrentCooldown = newCooldownTime;
                m_CooldownText.SetText(mCurrentCooldown.ToString(mCooldownFormat));
                break;
            case SkillButtonStates.Disabled:
                break;
            default:
                break;
        }

        mButtonState = newButtonState;
    }

    // Updates button each frame
    private void UpdateButtonState()
    {
        switch (mButtonState)
        {
            case SkillButtonStates.Available:
                break;
            case SkillButtonStates.Cooldown:
                
                mCurrentCooldown -= Time.deltaTime;
                m_CooldownText.SetText(mCurrentCooldown.ToString(mCooldownFormat));
                
                if (mCurrentCooldown <= 0)
                {
                    SetButtonState(SkillButtonStates.Available);
                }

                break;
            case SkillButtonStates.Disabled:
                break;
            default:
                break;
        }
    }

    // External change button state function. Use this to change state from other scripts
    public void ChangeButtonState(SkillButtonStates newButtonState, float newCooldownTime = 0)
    {
        SetButtonState(newButtonState, newCooldownTime);
    }
}
