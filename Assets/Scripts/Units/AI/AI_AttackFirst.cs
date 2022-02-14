using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_AttackFirst : EnemyAI
{
    public override float Act(EnemyUnit owner) {
        float timeInterval = 0;
        foreach (AttackAction attack in owner.attackList) {
            Condition attackCondition = attack.GetComponent<Condition>();
            if (owner.AP_current > attack.AP_cost && attackCondition.CheckConditions(owner)) {
                timeInterval += enemyAttackPriority.UseAttack(attack);
                break;
            }
        }

        foreach (SupportAction support in owner.supportList) {
            Condition supportCondition = support.GetComponent<Condition>();
            if (owner.AP_current > support.AP_cost && supportCondition.CheckConditions(owner)) {
                timeInterval += enemySupportPriority.UseSupport(support, "level");
            }
        }
        return timeInterval;
    }
}
