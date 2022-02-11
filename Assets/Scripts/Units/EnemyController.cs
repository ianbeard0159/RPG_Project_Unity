using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController
{
    private EnemyUnit unit;
    public EnemyController(EnemyUnit _unit) {
        unit = _unit;
    }

    public int TakeEnemyTurn()
    {
        unit.unitController.StartTurn();

        return (int)(unit.AI_instance.Act(unit));

    }

}
