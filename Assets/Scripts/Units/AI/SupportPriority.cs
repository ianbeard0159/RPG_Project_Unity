using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTargetEntry : IComparable {
    public Unit target;
    public Action action;
    public double duration;
    public string compareStat;
    public ActionTargetEntry(Unit _unit, Action _action, string _compareStat, double _duration) {
        target = _unit;
        action = _action;
        duration = _duration;
        compareStat = _compareStat;
    }
    public int CompareTo(object obj) {
        ActionTargetEntry otherUnit = obj as ActionTargetEntry;
        return compareStat.CompareTo(otherUnit.compareStat);
    }
}

public class SupportPriority
{
    EnemyUnit owner;
    bool isInit = false;

    public SupportPriority(EnemyUnit _owner) {
        owner = _owner;
    }

    private void Init() {
        isInit = true;
    }

    public static void CheckDuration() {
        for (int i = GameDriver.Instance.targetRegister.Count - 1; i >= 0; i--) {
            if (GameDriver.Instance.targetRegister[i].duration != -1) GameDriver.Instance.targetRegister[i].duration -= 1;
            if (GameDriver.Instance.targetRegister[i].duration == 0) GameDriver.Instance.targetRegister.RemoveAt(i);
        }
    }

    public float UseSupport(Action _action, string _compareStat) {
        List<Unit> selectedTargets = SupportTargetSelect(_action, _compareStat, GameDriver.Instance.enemyList);
        if (selectedTargets.Count == 0) {
            return 0;
        }
        else if (selectedTargets.Count == 1) {
            return GameDriver.Instance.PerformAction(selectedTargets[0], owner, _action);
        }
        else {
            return GameDriver.Instance.PerformAction(selectedTargets, owner, _action);
        }
    }

    private List<Unit> SupportTargetSelect(Action _action, string _compareStat, List<Unit> _potentialTargets) {
        if (!isInit) Init();
        List<Unit> selectedTargets = new List<Unit>();

        // Create ActionTargetEntry objects for each potential target, and sort them by the input stat
        List<ActionTargetEntry> potentialTargetEntries = new List<ActionTargetEntry>();
        foreach (Unit unit in _potentialTargets) {
            potentialTargetEntries.Add(new ActionTargetEntry(unit, _action, _compareStat, 3));
        }
        potentialTargetEntries.Sort();

        // Select targets that haven't been targeted by the input action recently
        for (int i = 0; i < _action.numTargets; i++) {
            if (i >= potentialTargetEntries.Count) {
                break;
            }
            foreach (ActionTargetEntry _entry in potentialTargetEntries) {
                if(!GameDriver.Instance.targetRegister.Any(entry => entry.target == _entry.target)) {
                    selectedTargets.Add(_entry.target);
                    GameDriver.Instance.targetRegister.Add(_entry);
                    break;
                }
            }
            
        }

        return selectedTargets;
    }
}
