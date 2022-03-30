using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    IUnit Owner { get; set; }

    double AP_cost { get; }
    double ESS_cost { get; }
    string AttackType { get; }
    double BaseAccuracy { get; }
    double CritChance { get; }
    double BaseDamage { get; }
    double NumTargets { get; }
    double NumHits { get; }
    double BaseAggro { get; }
    double IgnoreDefenseChance { get; }

    DamageDealt DealDamage(IUnit target);
}
