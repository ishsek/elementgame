using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Mage : Enemy
{
    [Header("Mage Specific Attack Settings")]
    [SerializeField] private GameObject m_AimIndicatorPrefab;
    [SerializeField] private GameObject m_AttackPrefab;
    [SerializeField] private float m_AttackDuration;
    private Vector3 AimLocation;
    private GameObject mAimIndicator;
    private enum AttackStage
    {
        Charging,
        Aiming,
        Firing,
        Recovering,
    }
    private AttackStage mCurrentStage = AttackStage.Charging;

    public override void SetStun(float duration)
    {
        base.SetStun(duration);
        mCurrentStage = AttackStage.Charging;
        if (mAimIndicator != null )
        {
            Destroy(mAimIndicator);
        }
    }

    public void Aim()
    {
        mAimIndicator = Instantiate(m_AimIndicatorPrefab, target.position, transform.rotation);
        mCurrentStage = AttackStage.Aiming;
}

    public void LockedOn()
    {
        mCurrentStage = AttackStage.Firing;
    }

    public void Fire()
    {
        GameObject AttackObject = Instantiate(m_AttackPrefab, mAimIndicator.transform.position, transform.rotation);
        Destroy(mAimIndicator);
        Destroy(AttackObject, m_AttackDuration);
        mCurrentStage = AttackStage.Recovering;
    }

    protected override void DoAttack()
    {
        switch (mCurrentStage)
        {
            case AttackStage.Charging:

                break;

            case AttackStage.Aiming:
                LocatePlayer();
                RotateToPlayer();

                AimLocation = target.position;
                AimLocation.y = 0;
                mAimIndicator.transform.position = AimLocation;
                break;

            case AttackStage.Firing:

                break;

            case AttackStage.Recovering:

                break;
        }
    }

}
