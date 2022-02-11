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
    List<ActionTargetEntry> targetRegister;
    bool isInit = false;

    public SupportPriority(EnemyUnit _owner) {
        owner = _owner;
    }

    private void Init() {
        targetRegister = new List<ActionTargetEntry>();
        isInit = true;
    }

    public void CheckDuration() {
        if (!isInit) Init();
        for (int i = targetRegister.Count - 1; i >= 0; i--) {
            if (targetRegister[i].duration != -1) targetRegister[i].duration -= 1;
            if (targetRegister[i].duration == 0) targetRegister.RemoveAt(i);
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
        foreach(ActionTargetEntry entry in targetRegister) {
            Debug.Log(entry.target.name);
        }

        // Select targets that haven't been targeted by the input action recently
        for (int i = 0; i < _action.numTargets; i++) {
            if (i >= potentialTargetEntries.Count) {
                break;
            }
            foreach (ActionTargetEntry _entry in potentialTargetEntries) {
                if(!targetRegister.Any(entry => entry.target == _entry.target)) {
                    selectedTargets.Add(_entry.target);
                    targetRegister.Add(_entry);
                    break;
                }
            }
            
        }

        return selectedTargets;
    }
}
