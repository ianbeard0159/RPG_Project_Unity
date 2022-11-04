using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Condition : MonoBehaviour
{
    public bool fullHealth;
    public bool lowHealth;


    public bool CheckConditions(Unit _unit) {
        ConditionItem[] conditionArray = {
            new ConditionItem(fullHealth, (_unit.HP_status == "full")),
            new ConditionItem(lowHealth, (_unit.HP_status == "low" || _unit.HP_status == "very low" || _unit.HP_status == "incapacitiated"))
        };
        bool result = true;
        foreach (ConditionItem item in conditionArray) if (item.activeCondition && !item.unitCondition) {
            result = false;
            break;
        }

        return result;
    }
}
