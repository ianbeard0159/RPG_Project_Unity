using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : Unit
{
    [SerializeField] public EnemyAI AI_instance;
    public AttackPriority enemyAggro { get; set; }
    public EnemyController enemyController;

    void Awake()
    {
        unitController = new UnitController(this);
        enemyController = new EnemyController(this);
        AI_instance = gameObject.GetComponent<EnemyAI>();
        unitController.Init();
    }
}
 