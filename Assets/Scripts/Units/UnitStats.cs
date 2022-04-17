using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [SerializeField] private double _level;
    public double level {
        get { return _level; }
        set { _level = value; }
    }
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
    [SerializeField] private double _HP_ratio;
    public double HP_ratio {
        get { return _HP_ratio; }
        set { _HP_ratio = value; }
    }
    [SerializeField] private double _AP_ratio;
    public double AP_ratio {
        get { return _AP_ratio; }
        set { _AP_ratio = value; }
    }
    [SerializeField] private double _ESS_ratio;
    public double ESS_ratio {
        get { return _ESS_ratio; }
        set { _ESS_ratio = value; }
    }

    [SerializeField] private Dictionary<string, double> _statBonuses = new Dictionary<string, double>() {
        {"strength", 0},
        {"willpower", 0},
        {"dexterity", 0},
        {"focus", 0},
        {"endurance", 0},
        {"agility", 0}
    };
    public Dictionary<string, double> statBonuses {
        get { return _statBonuses; }
        set { _statBonuses = value; }
    }

    private Dictionary<string, double> _damageBonuses = new Dictionary<string, double>() {
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
    public Dictionary<string, double> damageBonuses  { 
        get { return _damageBonuses; }
        set { _damageBonuses = value; }
    }

    private Dictionary<string, double> _rateBonuses = new Dictionary<string, double>() {
        {"accuracy", 0},
        {"physical", 0},
        {"magical", 0},
        {"critical", 0},
        {"evasion", 0},
        {"block", 0},
        {"counter", 0},
        {"intercept", 0}
    };
    public Dictionary<string, double> rateBonuses {
        get { return _rateBonuses; }
        set { _rateBonuses = value; }
    }
    // Resistances

    private Dictionary<string, double> _damageResistances = new Dictionary<string, double>() {
        {"damage", 0},
        {"physical", 0},
        {"magical", 0},
        {"fire", 0},
        {"earth", 0},
        {"wind", 0},
        {"electric", 0},
        {"water", 0}
    };
    public Dictionary<string, double> damageResistances {
        get { return _damageResistances; }
        set { _damageResistances = value; }
    }

    private Dictionary<string, double> _ailmentResistances = new Dictionary<string, double>() {
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
    public Dictionary<string, double> ailmentResistances {
        get { return _ailmentResistances; }
        set { _ailmentResistances = value; }
    }

}
