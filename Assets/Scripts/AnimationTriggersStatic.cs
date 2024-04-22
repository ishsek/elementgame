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

    // Shadow
    private static string EnterShadowMode = "EnterShadowMode";
    public static string GetEnterShadowMode() { return EnterShadowMode; }

    private static string ShadowDodge = "ShadowDodge";
    public static string GetShadowDodge() { return ShadowDodge; }

    private static string ShadowBlackhole = "ShadowBlackhole";
    public static string GetShadowBlackhole() { return ShadowBlackhole; }

    // Enemy
    private static string EnemyAttackTrigger = "Punch";
    public static string GetEnemyAttackTrigger() { return EnemyAttackTrigger; }

    private static string EnemyRunTrigger = "Run";
    public static string GetEnemyRunTrigger() { return EnemyRunTrigger; }

    private static string EnemyIdleTrigger = "Idle";
    public static string GetEnemyIdleTrigger() { return EnemyIdleTrigger; }
}
