using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : Action
{

    public string attackType;
    public double baseAccuracy;
    public double critChance;
    public double baseDamage;
    public double numHits;
    public double baseAggro;
    public bool ignoreEvasion;
    public bool ignoreBlocking;
    public bool ignoreCountering;


    private double DidHit(double _myAimStat, double _myTN, double _targetAgility, double _targetTN, double _typeRateBonus)
    {
        // Calculate hit chance
        double bonus = _typeRateBonus.EnforceRange(0.9, -0.9);
        double totalAccuracy = baseAccuracy + (baseAccuracy * bonus);
        double hitChance = Math.Ceiling(totalAccuracy + (_myAimStat * _myTN) - (_targetAgility * _targetTN));
        hitChance = hitChance.EnforceRange(99, 0);
        
        // Roll two numbers against hit chance
        int rollA = UnityEngine.Random.Range(1, 101);
        int rollB = UnityEngine.Random.Range(1, 101);
        // Return damage multiplier based on the two hit rolls
        if (hitChance >= rollA && hitChance >= rollB)
        {
            return 1.1;
        }
        else if (hitChance >= rollA || hitChance >= rollB)
        {
            return 1.0;
        }
        else
        {
            return 0;
        }

    }

    private double DidCrit(double _rateBonus, double _damageBonus)
    {
        // Roll a single random number against the crit chance
        int roll = UnityEngine.Random.Range(1, 101);

        // Multiply the damage by 1.5 if the attack scored a critical hit
        double totalCritChance = critChance + (critChance * _rateBonus);
        totalCritChance = totalCritChance.EnforceRange(99, 0);
        if (totalCritChance >= roll)
        {
            return 1.5 + _damageBonus;
        }
        else
        {
            return 1;
        }

    }

    private double CalculateDamage(double _hit, double _crit, double _myAttStat, double _myLevel, double _targetDef, double _damageBonus)
    {
        // Calculate the total damage dealt by the attack
        double damage = _hit * _crit * Math.Sqrt(_myAttStat / _targetDef) * baseDamage * (_myLevel / 2);
        damage = damage + (damage * _damageBonus);
        return damage;
    }
    public DamageDealt DealDamage(UnitStats _targetStats, double _TN_target, UnitStats _ownerStats, double _TN_owner)
    {
        // Identify the correct attack stat
        double myAttackStat;
        double myAimStat;
        double typeRateBonus;
        double typeDamageBonus;

        if (attackType == "physical") 
        { 
            myAttackStat = _ownerStats.strength;
            myAimStat = _ownerStats.dexterity;
            typeRateBonus = _ownerStats.rateBonuses["physical"] + _ownerStats.rateBonuses["accuracy"];
            typeDamageBonus = _ownerStats.damageBonuses["damage"] + _ownerStats.damageBonuses["physical"];
        }
        else
        {
            myAttackStat = _ownerStats.willpower;
            myAimStat = _ownerStats.focus;
            typeRateBonus = _ownerStats.rateBonuses["magical"] + _ownerStats.rateBonuses["accuracy"];
            typeDamageBonus = _ownerStats.damageBonuses["damage"] + _ownerStats.damageBonuses["magical"] + _ownerStats.damageBonuses[attackType];
        }
        double hit = DidHit(myAimStat, _TN_owner, _targetStats.agility, _TN_target, typeRateBonus);
        double crit = DidCrit(_ownerStats.rateBonuses["critical"], _ownerStats.damageBonuses["critical"]);
        double damage = CalculateDamage(hit, crit, myAttackStat, _ownerStats.level, _targetStats.endurance, typeDamageBonus);

        DamageDealt damageData = new DamageDealt();
        damageData.damage = damage;
        damageData.crit = crit;
        damageData.aggro = baseAggro;
        damageData.type = attackType;
        damageData.ailment = (ailment) ? ailment : null;
        damageData.ailmentBuildup =  ailmentBuildup;
        damageData.ignoreEvasion = ignoreEvasion;
        damageData.ignoreBlock = ignoreBlocking;
        damageData.ignoreCounter = ignoreCountering;

        return damageData;

    }

}
