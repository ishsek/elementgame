using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationTriggersStatic
{
    // Player
    private static string PlayerAttack1 = "SwordSlash1";
    public static string GetPlayerAttack1() { return PlayerAttack1; }

    private static string PlayerAttack2 = "SwordSlash2";
    public static string GetPlayerAttack2() { return PlayerAttack2; }

    private static string PlayerAttack3 = "SwordSlash3";
    public static string GetPlayerAttack3() { return PlayerAttack3; }

    private static string InterruptToIdle = "InterruptAttack";
    public static string GetInterruptToIdle() { return InterruptToIdle; }

    private static string DisableElement = "DisableElement";
    public static string GetDisableElement() { return DisableElement; }

    // Shadow
    private static string EnterShadowMode = "EnterShadowMode";
    public static string GetEnterShadowMode() { return EnterShadowMode; }

    private static string ShadowDodge = "ShadowDodge";
    public static string GetShadowDodge() { return ShadowDodge; }

    private static string ShadowHeavy1 = "ShadowHeavy1";
    public static string GetShadowHeavy1() { return ShadowHeavy1; }

    private static string ShadowHeavy2 = "ShadowHeavy2";
    public static string GetShadowHeavy2() { return ShadowHeavy2; }

    private static string ShadowProjectile = "ShadowProjectile";
    public static string GetShadowProjectile() { return ShadowProjectile; }

    private static string ShadowBlackholeCharge = "ShadowBlackholeCharge";
    public static string GetShadowBlackholeCharge() { return ShadowBlackholeCharge; }

    private static string ShadowBlackholeFire = "ShadowBlackholeFire";
    public static string GetShadowBlackholeFire() { return ShadowBlackholeFire; }

    private static string ShadowStabCharge = "ShadowStabCharge";
    public static string GetShadowStabCharge() { return ShadowStabCharge; }

    private static string ShadowStabFire = "ShadowStabFire";
    public static string GetShadowStabFire() { return ShadowStabFire; }

    private static string ShadowDashCharge = "ShadowDashCharge";
    public static string GetShadowDashCharge() { return ShadowDashCharge; }

    private static string ShadowDashFire = "ShadowDashFire";
    public static string GetShadowDashFire() { return ShadowDashFire; }

    private static string ShadowHeal = "ShadowHeal";
    public static string GetShadowHeal() { return ShadowHeal; }

    // Enemy
    private static string EnemyAttackTrigger = "Attack";
    public static string GetEnemyAttackTrigger() { return EnemyAttackTrigger; }

    private static string EnemyRunTrigger = "Run";
    public static string GetEnemyRunTrigger() { return EnemyRunTrigger; }

    private static string EnemyIdleTrigger = "Idle";
    public static string GetEnemyIdleTrigger() { return EnemyIdleTrigger; }

    private static string EnemyDeathTrigger = "Death";
    public static string GetEnemyDeathTrigger() { return EnemyDeathTrigger; }

    private static string EnemyInterruptTrigger = "Interrupt";
    public static string GetEnemyInterruptTrigger() { return EnemyInterruptTrigger; }
}
