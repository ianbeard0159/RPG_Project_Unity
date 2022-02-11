using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_AttackFirst : EnemyAI
{
    public override float Act(EnemyUnit owner) {
        float timeInterval = 0;
        foreach (AttackAction attack in owner.attackList) {
            if (owner.AP_current > attack.AP_cost) {
                timeInterval += enemyAttackPriority.UseAttack(attack);
                break;
            }
        }

        enemySupportPriority.CheckDuration();
        foreach (SupportAction support in owner.supportList) {
            if (owner.AP_current > support.AP_cost) {
                timeInterval += enemySupportPriority.UseSupport(support, "level");
            }
        }
        return timeInterval;
    }
}
