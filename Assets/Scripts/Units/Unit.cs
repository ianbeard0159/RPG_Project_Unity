using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IComparable
{
    public UnitController unitController;

    // Stats
    [SerializeField] private double _strength;
    public double strength {
        get { return _strength + statBonuses["strength"]; }
        private set { _strength = value; }
    }
    [SerializeField] private double _willpower;
    public double willpower {
        get { return _willpower + statBonuses["willpower"]; }
        private set { _willpower = value; }
    }
    [SerializeField] private double _dexterity;
    public double dexterity {
        get { return _dexterity + statBonuses["dexterity"]; }
        private set { _dexterity = value; }
    }
    [SerializeField] private double _focus;
    public double focus {
        get { return _focus + statBonuses["focus"]; }
        private set { _focus = value; }
    }
    [SerializeField] private double _endurance;
    public double endurance {
        get { return _endurance + statBonuses["endurance"]; }
        private set { _endurance = value; }
    }
    [SerializeField] private double _agility;
    public double agility {
        get { return _agility + statBonuses["agility"]; }
        private set { _agility = value; }
    }

    // Resources

    public double HP_ratio;
    public double AP_ratio;
    public double ESS_ratio;
    public double level;

    // Bonuses

    public Dictionary<string, double> statBonuses = new Dictionary<string, double>() {
        {"strength", 0},
        {"willpower", 0},
        {"dexterity", 0},
        {"focus", 0},
        {"endurance", 0},
        {"agility", 0}
    };

    public Dictionary<string, double> damageBonuses { get; set; }

    public Dictionary<string, double> rateBonuses { get; set; }

    // Resistances

    public Dictionary<string, double> damageResistances { get; set; }

    public Dictionary<string, double> ailmentResistances { get; set; }

    // Actions

    [SerializeField] private List<Action> assignedAttacks;
    [SerializeField] private List<Action> assignedSupports;
    public List<Action> attackList { get; protected set; }
    public List<Action> supportList { get; protected set; }
    public List<Modifier> modifierList;

    private double _evasion;
    public double evasion { 
        get { 
            double returnVal = _evasion + (_evasion * rateBonuses["evasion"]);
            returnVal = returnVal.EnforceRange(75, 0);
            return Math.Round(returnVal); 
        } 
        set { _evasion = value; } 
    }
    
    private double _block;
    public double block { 
        get { 
            double returnVal = _block + (_block * rateBonuses["block"]); 
            returnVal = returnVal.EnforceRange(75, 0);
            return Math.Round(returnVal);
        } 
        set { _block = value; } 
    }
    
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
        return agility.CompareTo(otherUnit.agility);
    }

    public void AssignAttacks() {
        // Assign attacks
        attackList = new List<Action>();
        foreach (AttackAction attack in assignedAttacks) {
            // Make a unique instance of the attack action
            AttackAction newAttack = Instantiate(attack, this.gameObject.transform);
            newAttack.owner = this;
            newAttack.name = newAttack.name.Replace("(Clone)", "");
            attackList.Add(newAttack);
        }

    }

    public void AssignSupports() {
        // Assign Supports
        supportList = new List<Action>();
        foreach (SupportAction support in assignedSupports) {
            // Make a unique instance of the support action
            SupportAction newSupport = Instantiate(support, this.gameObject.transform);
            newSupport.owner = this;
            newSupport.name = newSupport.name.Replace("(Clone)", "");
            supportList.Add(newSupport);
        }
    }

}
