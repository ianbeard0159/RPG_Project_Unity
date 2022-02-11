using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPriority {
    public EnemyUnit owner;
    public List<AggroTableEntry> aggroTable;

    public AttackPriority(EnemyUnit _owner) {
        owner = _owner;
        aggroTable = new List<AggroTableEntry>();
    }
    public void PopulateAggroTable(List<Unit> characterList)
    {
        foreach (CharacterUnit character in characterList)
        {
            AggroTableEntry newEntry = new AggroTableEntry(character);
            aggroTable.Add(newEntry);
        }

    }
    public void SortAggroTable() {
        aggroTable.Sort();
        aggroTable.Reverse();
    }

    public void UpdateAggroTable(CharacterUnit _character, double value) {
        AggroTableEntry entry = aggroTable.Find(_entry => _entry.character == _character);
        entry.aggro += value;
        SortAggroTable();
    }

    public float UseAttack(Action selectedAttack) {
        // Select Targets
        List<Unit> selectedTargets = EnemyTargetSelect(selectedAttack.numTargets);

        if(selectedTargets.Count == 1) {
            return GameDriver.Instance.PerformAction(selectedTargets[0], owner, selectedAttack);
        }
        else {
            return GameDriver.Instance.PerformAction(selectedTargets, owner, selectedAttack);
        }
    }
    private List<Unit> EnemyTargetSelect(double numTargets)
    {
        List<Unit> targets = new List<Unit>();
        for (int i = 0; i < numTargets; i++)
        {
            if (i >= aggroTable.Count) break;
            else targets.Add(aggroTable[i].character);
        }
        return targets;
    }
}
