using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAI : MonoBehaviour
{
    public SupportPriority enemySupportPriority;
    public AttackPriority enemyAttackPriority;

    void Awake() {
        enemySupportPriority = new SupportPriority(gameObject.GetComponent<EnemyUnit>());
        enemyAttackPriority = new AttackPriority(gameObject.GetComponent<EnemyUnit>());
    }
    public abstract float Act(EnemyUnit owner);
}
