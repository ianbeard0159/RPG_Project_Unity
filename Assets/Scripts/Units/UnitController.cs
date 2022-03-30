using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitController
{
    private Unit unit;
    private Transform HP_text;
    private Transform ESS_text;
    private Transform AP_text;
    private Transform TN_text;

    public UnitController(Unit _unit)
    {
        unit = _unit;
    }

    public void Init()
    {
        // Initialize calculated stats
        unit.HP_max = unit.HP_ratio * Math.Round(Math.Sqrt(unit.level));
        unit.AP_max = unit.AP_ratio + Math.Round(Math.Sqrt(unit.level));
        unit.ESS_max = unit.ESS_ratio * Math.Round(Math.Sqrt(unit.level));
        unit.evasion = Math.Round(((unit.agility - 10) / 2));
        unit.block = Math.Round((unit.endurance - 10) / 2);

        // Initialize starting resources
        unit.HP_current = unit.HP_max;
        unit.AP_current = unit.AP_max;
        unit.ESS_current = unit.ESS_max;
        unit.TN_current = 1.0;

        // Set status flags
        unit.HP_status = "full";
        unit.TN_status = "normal";

        // Assign attacks
        unit.AssignAttacks();
        unit.AssignSupports();
        unit.modifierList = new List<Modifier>();
        unit.ailmentList = new List<StatusAilment>();

        // Initialize on screen text
        HP_text = unit.gameObject.transform.Find("HP_text");
        HP_text.GetComponent<Text>().text = "HP: " + unit.HP_current;
        ESS_text = unit.gameObject.transform.Find("ESS_text");
        ESS_text.GetComponent<Text>().text = "ESS: " + unit.ESS_current;
        AP_text = unit.gameObject.transform.Find("AP_text");
        AP_text.GetComponent<Text>().text = "AP: " + unit.AP_current;
        TN_text = unit.gameObject.transform.Find("TN_text");
        TN_text.GetComponent<Text>().text = "TN: " + unit.TN_current;

        // Set up dictionaries
        unit.damageBonuses = new Dictionary<string, double>() {
            {"damage", 0},
            {"physical", 0},
            {"magical", 0},
            {"fire", 0},
            {"earth", 0},
            {"wind", 0},
            {"electric", 0},
            {"water", 0},
            {"critical", 0}
        };
        unit.rateBonuses = new Dictionary<string, double>() {
            {"accuracy", 0},
            {"physical", 0},
            {"magical", 0},
            {"critical", 0},
            {"evasion", 0},
            {"block", 0},
            {"counter", 0},
            {"intercept", 0}
        };
        unit.damageResistances = new Dictionary<string, double>() {
            {"damage", 0},
            {"physical", 0},
            {"magical", 0},
            {"fire", 0},
            {"earth", 0},
            {"wind", 0},
            {"electric", 0},
            {"water", 0}
        };
        unit.ailmentResistances = new Dictionary<string, double>() {
            {"mundane", 0},
                {"stagger", 0},
                {"bleed", 0},
                {"stun", 0},
                {"poison", 0},
                {"fatigue", 0},
            {"mental", 0},
                {"fear", 0},
                {"despair", 0},
                {"confusion", 0},
                {"anger", 0},
                {"pressure", 0},
            {"elemental", 0},
                {"burn", 0},
                {"harden", 0},
                {"suffocate", 0},
                {"shock", 0},
                {"chill", 0}
        };
        UpdateModifiers(unit.modifierList);
    }
    
    public void StartTurn() {
        // Generate Resources
        ChangeAP(Math.Round(unit.AP_max / 2));
        ChangeESS(Math.Round(unit.ESS_max / 10));
        
        List<Modifier> initialMods = new List<Modifier>(unit.modifierList);
        ChangeModifierDuration();
        ApplySpecialRules();
        for (int i = unit.ailmentList.Count; i > 0; i--) {
            unit.ailmentList[i-1].RefreshAilment(unit);
        }
        UpdateModifiers(initialMods);

    }

    public void ChangeHealth(double value)
    {
        // Set new value
        unit.HP_current += value;
        unit.HP_current = unit.HP_current.EnforceRange(unit.HP_max, 0);
        
        // Set new status
        if (unit.HP_current >= unit.HP_max) unit.HP_status = "full";
        else if (unit.HP_current > unit.HP_max / 2) unit.HP_status = "high";
        else if (unit.HP_current > unit.HP_max / 4) unit.HP_status = "low";
        else if (unit.HP_current > 0) unit.HP_status = "very low";
        else unit.HP_status = "incapacitated";
        
        // Set on screen text
        HP_text.GetComponent<Text>().text = "HP: " + unit.HP_current;

        // Check if any special rules triggered
        List<Modifier> initialMods = new List<Modifier>(unit.modifierList);
        ApplySpecialRules();
        UpdateModifiers(initialMods);
    }
    public void ChangeTN(double change)
    {
        // Set new value
        unit.TN_current = Math.Round((unit.TN_current + change), 2);
        unit.TN_current = unit.TN_current.EnforceRange(1.5, 0.5);
        
        // Set new status
        if (unit.TN_current >= 1.5) unit.TN_status = "very high";
        else if (unit.TN_current > 1.3) unit.TN_status = "very high"; 
        else if (unit.TN_current > 1.1) unit.TN_status = "high"; 
        else if (unit.TN_current > 0.9) unit.TN_status = "normal"; 
        else if (unit.TN_current > 0.7) unit.TN_status = "low"; 
        else unit.TN_status = "very low"; 
        
        // Set on screen text
        TN_text.GetComponent<Text>().text = "TN: " + unit.TN_current;
        List<Modifier> initialMods = new List<Modifier>(unit.modifierList);
        ApplySpecialRules();
        UpdateModifiers(initialMods);
    }
    public double ChangeESS(double _change) {
        unit.ESS_current += _change;
        if (unit.ESS_current > unit.ESS_max) unit.ESS_current = unit.ESS_max;
        double burnDamage = 0;

        // Apply essence burn if the essence cost of the ability is greater than the unit's remaining AP
        if (unit.ESS_current < 0) {
            double burnPercent = (unit.ESS_current) / unit.ESS_max;
            burnDamage = Math.Round(burnPercent * unit.HP_max);
            unit.ESS_current = 0;
            ChangeHealth(burnDamage);
        }
        // Set onscreen text
        ESS_text.GetComponent<Text>().text = "ESS: " + unit.ESS_current;

        return burnDamage;
    }
    public void ChangeAP(double _change) {
        unit.AP_current = (unit.AP_current + _change).EnforceRange(unit.AP_max, 0);
        // Set onscreen text
        AP_text.GetComponent<Text>().text = "AP: " + unit.AP_current;
    }
    public double SpendResources(double _AP_cost, double _ESS_cost) {
        ChangeAP(-_AP_cost);
        return ChangeESS(-_ESS_cost);
    }

    // Receive damage from attacks
    public DamageTaken TakeDamage(DamageDealt _damageData)
    {
        DamageTaken takeData = new DamageTaken();
        
        // Apply Resistances
        double resistances;
        double resMax = 0.9;
        double resMin = -0.9;
        if (_damageData.type == "physical") {
            resistances = unit.damageResistances["damage"] + unit.damageResistances["physical"];
        }
        else {
            resistances = unit.damageResistances["damage"] + unit.damageResistances["magical"] + unit.damageResistances[_damageData.type];
        }
        resistances = resistances.EnforceRange(resMax, resMin);
        takeData.damage = Math.Round(_damageData.damage - (_damageData.damage * resistances));

        // Calculate potential tension change
        double HP_percent = takeData.damage / unit.HP_max;
        takeData.TN_change = Math.Ceiling((HP_percent / 10) * 100) / 100;

        // Attempt to defend from the attack and apply takeData changes
        // Returns true if attack was fully defended, false otherwise
        Func<bool> AttemptDefenses = () => {
            
            // Check if the attack missed
            if (takeData.damage == 0)
            {
                takeData.TN_change = -0.01;
                takeData.result = "miss";
                return true;
            }

            // Check if the attack was evaded
            if (_damageData.evadeable) 
            {
                int rollA = UnityEngine.Random.Range(1, 100);
                int rollB = UnityEngine.Random.Range(1, 100);
                // Attack evaded if either number is below the total evasion chance
                if (unit.evasion > rollA || unit.evasion > rollB)
                {
                    takeData.damage = 0;
                    takeData.result = "evaded";
                    return true;
                }
            }

            // Check if the attack was blocked
            if (_damageData.blockable) {
                int rollC = UnityEngine.Random.Range(1, 100);
                int rollD = UnityEngine.Random.Range(1, 100);
                // The attack is fully blocked of both numbers are below the block chance
                if (unit.block > rollC && unit.block > rollD)
                {
                    takeData.damage = 0;
                    takeData.result = "blocked";
                    return true;
                }
                // The attack is partially blocked if one number is below the block chance
                else if (unit.block > rollC || unit.block > rollD)
                {
                    takeData.damage = Math.Round(takeData.damage / 2);
                    takeData.TN_change = -takeData.TN_change / 2;
                    takeData.result = "partially blocked";
                    return false;
                }
            }

            // If all of the above fail, the attack hits
            takeData.TN_change = -takeData.TN_change;
            takeData.result = "taken";
            return false;
        };

        bool attackHit = !AttemptDefenses();

        // Set new values for health and tension
        ChangeHealth(-takeData.damage);
        ChangeTN(takeData.TN_change);

        // Apply Status Conditions if the Attack Hit
        if (_damageData.ailment != null && attackHit) {
            _damageData.ailment.AddAilment(unit, _damageData.ailmentBuildup);
        }
      
        // If an enemy target is incapacitated by the attack, destroy it
        if (unit.HP_status == "incapacitated" && unit.gameObject.tag == "EnemyUnit") {
            Debug.Log(unit.name);
            GameDriver.Instance.SelfDestruct(unit);
        }

        return takeData;
    }

    public void ResetModifers(Dictionary<string, double> _dictionary) {
        List<string> keys = new List<string>(_dictionary.Keys);
        foreach (string key in keys) {
            _dictionary[key] = 0;
        }
    }
    public void ChangeModifierDuration() {
        // Check the duration of modifiers in the mod list
        for (int i = unit.modifierList.Count - 1; i >= 0; i--) {
            if (unit.modifierList[i].duration != -1) {
                unit.modifierList[i].currentDuration -= 1;
                // Remove modifiers that have expired
                if (unit.modifierList[i].currentDuration == 0) {
                    unit.modifierList[i].SelfDestruct();
                    unit.modifierList.RemoveAt(i);
                }
            }
        }
    }
    public void ApplySpecialRules() {
        foreach (GameObject specialRule in unit.specialRules) {
            Condition ruleConditions = specialRule.GetComponent<Condition>();
            ModList ruleModifiers = specialRule.GetComponent<ModList>();
            foreach (Modifier mod in ruleModifiers.modifiers) {
                if (ruleConditions.CheckConditions(unit) && !(unit.modifierList.Contains(mod))) {
                    unit.modifierList.Add(mod);
                }
                else if (!ruleConditions.CheckConditions(unit) && unit.modifierList.Contains(mod)) {
                    unit.modifierList.Remove(mod);
                }

            }
        }

    }

    public void UpdateModifiers(List<Modifier> initialMods) {
        if (!initialMods.SequenceEqual(unit.modifierList)) {
            // Reset all modifiers to zero
            ResetModifers(unit.statBonuses);
            ResetModifers(unit.damageBonuses);
            ResetModifers(unit.rateBonuses);
            ResetModifers(unit.damageResistances);
            ResetModifers(unit.ailmentResistances);

            // Re-apply modifiers that are still assigned to the unit
            foreach (Modifier mod in unit.modifierList) {
                switch (mod.type) {
                    case "stat":
                        unit.statBonuses[mod.attribute] += mod.value;
                        break;
                    case "damage":
                        unit.damageBonuses[mod.attribute] += mod.value;
                        break;
                    case "rate":
                        unit.rateBonuses[mod.attribute] += mod.value;
                        break;
                    case "resistance":
                        unit.damageResistances[mod.attribute] += mod.value;
                        break;
                    case "ailment":
                        unit.ailmentResistances[mod.attribute] += mod.value;
                        break;
                }
            }

        }
    }
}
