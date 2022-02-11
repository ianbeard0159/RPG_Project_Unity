using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPriority : IComparable
{
    Unit target;
    Action selectedAction;
    int duration;
    double compareStat;
    public int CompareTo(object obj) {
        ActionPriority otherUnit = obj as ActionPriority;
        return compareStat.CompareTo(otherUnit.compareStat);
    }
}
