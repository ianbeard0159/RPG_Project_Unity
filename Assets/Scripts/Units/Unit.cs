using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public abstract class Unit : MonoBehaviour, IComparable
{
    public UnitStats unitStats { get; set; }


    [SerializeField] public List<GameObject> specialRules;
    [SerializeField] public List<Action> attackList;
    [SerializeField] public List<Action> supportList;
    [SerializeField] public List<Modifier> modifierList;
    [SerializeField] public List<StatusAilment> ailmentList;

    private double _evasion;
    public double evasion { 
        get { 
            double returnVal = _evasion + (_evasion * unitStats.rateBonuses["evasion"]);
            returnVal = returnVal.EnforceRange(75, 0);
            return Math.Round(returnVal); 
        } 
        set { _evasion = value; } 
    }
    
    private double _block;
    public double block { 
        get { 
            double returnVal = _block + (_block * unitStats.rateBonuses["block"]); 
            returnVal = returnVal.EnforceRange(75, 0);
            return Math.Round(returnVal);
        } 
        set { _block = value; } 
    }
    // Resources
    private Transform HP_text;
    private Transform ESS_text;
    private Transform AP_text;
    private Transform TN_text;
    
    public double HP_max { get; set; }
    public double HP_current { get; set; }
    public string HP_status { get; set; }

    public double AP_max { get; set; }
    public double AP_current { get; set; }
    public string AP_status { get; set; }

    public double ESS_max { get; set; }
    public double ESS_current { get; set; }
    public string ESS_status { get; set; }

    public double TN_current { get; set; }
    public string TN_status { get; set; }


    public int CompareTo(object obj)
    {
        // Use the unit's agility when sorting units in a list
        Unit otherUnit = obj as Unit;
        return unitStats.agility.CompareTo(otherUnit.unitStats.agility);
    }

    protected void Init()
    {
        unitStats = this.gameObject.GetComponent<UnitStats>();

        // Initialize calculated stats
        HP_max = unitStats.HP_ratio * Math.Round(Math.Sqrt(unitStats.level));
        AP_max = unitStats.AP_ratio + Math.Round(Math.Sqrt(unitStats.level));
        ESS_max = unitStats.ESS_ratio * Math.Round(Math.Sqrt(unitStats.level));
        evasion = Math.Round(((unitStats.agility - 10) / 2));
        block = Math.Round((unitStats.endurance - 10) / 2);

        // Initialize starting resources
        HP_current = HP_max;
        AP_current = AP_max;
        ESS_current = ESS_max;
        TN_current = 1.0;

        // Set status flags
        HP_status = "full";
        TN_status = "normal";

        // Initialize on screen text
        HP_text = gameObject.transform.Find("HP_text");
        HP_text.GetComponent<Text>().text = "HP: " + HP_current;
        ESS_text = gameObject.transform.Find("ESS_text");
        ESS_text.GetComponent<Text>().text = "ESS: " + ESS_current;
        AP_text = gameObject.transform.Find("AP_text");
        AP_text.GetComponent<Text>().text = "AP: " + AP_current;
        TN_text = gameObject.transform.Find("TN_text");
        TN_text.GetComponent<Text>().text = "TN: " + TN_current;

        // Set up dictionaries
        
        
        
        UpdateModifiers(modifierList);
    }
    
    public void StartTurn() {
        // Generate Resources
        ChangeAP(Math.Round(AP_max / 2));
        ChangeESS(Math.Round(ESS_max / 10));
        
        List<Modifier> initialMods = new List<Modifier>(modifierList);
        ChangeModifierDuration();
        ApplySpecialRules();
        for (int i = ailmentList.Count; i > 0; i--) {
            ailmentList[i-1].RefreshAilment(this);
        }
        UpdateModifiers(initialMods);

    }

    public void ChangeHealth(double value)
    {
        // Set new value
        HP_current += value;
        HP_current = HP_current.EnforceRange(HP_max, 0);
        
        // Set new status
        if (HP_current >= HP_max) HP_status = "full";
        else if (HP_current > HP_max / 2) HP_status = "high";
        else if (HP_current > HP_max / 4) HP_status = "low";
        else if (HP_current > 0) HP_status = "very low";
        else HP_status = "incapacitated";
        
        // Set on screen text
        HP_text.GetComponent<Text>().text = "HP: " + HP_current;

        // Check if any special rules triggered
        List<Modifier> initialMods = new List<Modifier>(modifierList);
        ApplySpecialRules();
        UpdateModifiers(initialMods);
    }
    public void ChangeTN(double change)
    {
        // Set new value
        TN_current = Math.Round((TN_current + change), 2);
        TN_current = TN_current.EnforceRange(1.5, 0.5);
        
        // Set new status
        if (TN_current >= 1.5) TN_status = "very high";
        else if (TN_current > 1.3) TN_status = "very high"; 
        else if (TN_current > 1.1) TN_status = "high"; 
        else if (TN_current > 0.9) TN_status = "normal"; 
        else if (TN_current > 0.7) TN_status = "low"; 
        else TN_status = "very low"; 
        
        // Set on screen text
        TN_text.GetComponent<Text>().text = "TN: " + TN_current;
        List<Modifier> initialMods = new List<Modifier>(modifierList);
        ApplySpecialRules();
        UpdateModifiers(initialMods);
    }
    public double ChangeESS(double _change) {
        ESS_current += _change;
        if (ESS_current > ESS_max) ESS_current = ESS_max;
        double burnDamage = 0;

        // Apply essence burn if the essence cost of the ability is greater than the unit's remaining AP
        if (ESS_current < 0) {
            double burnPercent = (ESS_current) / ESS_max;
            burnDamage = Math.Round(burnPercent * HP_max);
            ESS_current = 0;
            ChangeHealth(burnDamage);
        }
        // Set onscreen text
        ESS_text.GetComponent<Text>().text = "ESS: " + ESS_current;

        return burnDamage;
    }
    public void ChangeAP(double _change) {
        AP_current = (AP_current + _change).EnforceRange(AP_max, 0);
        // Set onscreen text
        AP_text.GetComponent<Text>().text = "AP: " + AP_current;
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
            resistances = unitStats.damageResistances["damage"] + unitStats.damageResistances["physical"];
        }
        else {
            resistances = unitStats.damageResistances["damage"] + unitStats.damageResistances["magical"] + unitStats.damageResistances[_damageData.type];
        }
        resistances = resistances.EnforceRange(resMax, resMin);
        takeData.damage = Math.Round(_damageData.damage - (_damageData.damage * resistances));

        // Calculate potential tension change
        double HP_percent = takeData.damage / HP_max;
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
            if (!_damageData.ignoreEvasion) 
            {
                int rollA = UnityEngine.Random.Range(1, 100);
                int rollB = UnityEngine.Random.Range(1, 100);
                // Attack evaded if either number is below the total evasion chance
                if (evasion > rollA || evasion > rollB)
                {
                    takeData.damage = 0;
                    takeData.result = "evaded";
                    return true;
                }
            }

            // Check if the attack was blocked
            if (!_damageData.ignoreBlock) {
                int rollC = UnityEngine.Random.Range(1, 100);
                int rollD = UnityEngine.Random.Range(1, 100);
                // The attack is fully blocked of both numbers are below the block chance
                if (block > rollC && block > rollD)
                {
                    takeData.damage = 0;
                    takeData.result = "blocked";
                    return true;
                }
                // The attack is partially blocked if one number is below the block chance
                if (block > rollC || block > rollD)
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
            _damageData.ailment.AddAilment(this, _damageData.ailmentBuildup);
        }
      
        // If an enemy target is incapacitated by the attack, destroy it
        if (HP_status == "incapacitated" && gameObject.tag == "EnemyUnit") {
            Debug.Log(name);
            GameDriver.Instance.SelfDestruct(this);
        }

        return takeData;
    }

    private void ResetModifers(Dictionary<string, double> _dictionary) {
        List<string> keys = new List<string>(_dictionary.Keys);
        foreach (string key in keys) {
            _dictionary[key] = 0;
        }
    }
    private void ChangeModifierDuration() {
        // Check the duration of modifiers in the mod list
        for (int i = modifierList.Count - 1; i >= 0; i--) {
            if (modifierList[i].duration != -1) {
                modifierList[i].currentDuration -= 1;
                // Remove modifiers that have expired
                if (modifierList[i].currentDuration == 0) {
                    modifierList[i].SelfDestruct();
                    modifierList.RemoveAt(i);
                }
            }
        }
    }
    private void ApplySpecialRules() {
        foreach (GameObject specialRule in specialRules) {
            Condition ruleConditions = specialRule.GetComponent<Condition>();
            ModList ruleModifiers = specialRule.GetComponent<ModList>();
            foreach (Modifier mod in ruleModifiers.modifiers) {
                if (ruleConditions.CheckConditions(this) && !(modifierList.Contains(mod))) {
                    modifierList.Add(mod);
                }
                else if (!ruleConditions.CheckConditions(this) && modifierList.Contains(mod)) {
                    modifierList.Remove(mod);
                }

            }
        }

    }

    public void UpdateModifiers(List<Modifier> initialMods) {
        if (!initialMods.SequenceEqual(modifierList)) {
            // Reset all modifiers to zero
            ResetModifers(unitStats.statBonuses);
            ResetModifers(unitStats.damageBonuses);
            ResetModifers(unitStats.rateBonuses);
            ResetModifers(unitStats.damageResistances);
            ResetModifers(unitStats.ailmentResistances);

            // Re-apply modifiers that are still assigned to the unit
            foreach (Modifier mod in modifierList) {
                switch (mod.type) {
                    case "stat":
                        unitStats.statBonuses[mod.attribute] += mod.value;
                        break;
                    case "damage":
                        unitStats.damageBonuses[mod.attribute] += mod.value;
                        break;
                    case "rate":
                        unitStats.rateBonuses[mod.attribute] += mod.value;
                        break;
                    case "resistance":
                        unitStats.damageResistances[mod.attribute] += mod.value;
                        break;
                    case "ailment":
                        unitStats.ailmentResistances[mod.attribute] += mod.value;
                        break;
                }
            }

        }
    }

}
