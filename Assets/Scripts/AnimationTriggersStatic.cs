using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimationTriggersStatic
{
    // Player

    // Enemy
    private static string EnemyAttackTrigger = "Punch";
    public static string GetEnemyAttackTrigger() { return EnemyAttackTrigger; }

    private static string EnemyRunTrigger = "Run";
    public static string GetEnemyRunTrigger() { return EnemyRunTrigger; }

    private static string EnemyIdleTrigger = "Idle";
    public static string GetEnemyIdleTrigger() { return EnemyIdleTrigger; }
}
