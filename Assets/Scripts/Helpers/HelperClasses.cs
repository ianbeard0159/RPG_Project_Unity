using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

// Damage Data
public class DamageTaken {
    public double damage;
    public double TN_change;
    public string result;
}

public class DamageDealt {
    public double damage;
    public double crit;
    public double aggro;
    public string type;
    public StatusAilment ailment;
    public double ailmentBuildup;
    public bool ignoreEvasion;
    public bool ignoreBlock;
    public bool ignoreCounter;
}

public class ConditionItem {
    public bool activeCondition;
    public bool unitCondition;
    public ConditionItem(bool in_activeCondition, bool in_unitCondition) {
        activeCondition = in_activeCondition;
        unitCondition = in_unitCondition;
    }
}

static class SplitCamelCaseExtension {
	public static string SplitCamelCase(this string str) {
        // Capitalize First Letter
        if (str.Length > 1) {
            str = char.ToUpper(str[0]) + str.Substring(1);
        }
		return Regex.Replace( Regex.Replace( str, @"(\P{Ll})(\P{Ll}\p{Ll})", "$1 $2" ), @"(\p{Ll})(\P{Ll})", "$1 $2" );
	}
}

static class EnforeceRangeExtension {
    public static double EnforceRange(this double _value, double _max, double _min) {
        if (_value > _max) return _max;
        else if (_value < _min) return _min;
        else return _value;
    }
}


