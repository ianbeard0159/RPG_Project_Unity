using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    [SerializeField] public EnemyAI AI_instance;
    public AttackPriority enemyAggro { get; set; }

    void Awake()
    {
        AI_instance = gameObject.GetComponent<EnemyAI>();
        Init();
    }
    public int TakeEnemyTurn()
    {
        StartTurn();

        return (int)(AI_instance.Act(this));

    }
}
 