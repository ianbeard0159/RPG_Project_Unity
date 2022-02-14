using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConditionController
{
    //
    // Health Status
    //
    public static bool HealthIsFull(Unit _unit) {
        bool result = false;
        if (_unit.HP_status == "full") {
            result = true;
        }
        return result;
    }
    public static bool HealthIsHigh(Unit _unit) {
        bool result = false;
        if (_unit.HP_status == "high" || _unit.HP_status == "full") {
            result = true;
        }
        return result;
    }
    public static bool HealthIsLow(Unit _unit) {
        bool result = false;
        if (_unit.HP_status == "low" || _unit.HP_status == "very low" || _unit.HP_status == "incapacitiated") {
            result = true;
        }
        return result;
    }
    public static bool HealthIsVeryLow(Unit _unit) {
        bool result = false;
        if (_unit.HP_status == "very low" || _unit.HP_status == "incapacitiated") {
            result = true;
        }
        return result;
    }
}
