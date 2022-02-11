using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUnit
{
    double Strength { get; }
    double Willpower { get; }
    double Dexterity { get; }
    double Focus { get; }
    double Defense { get; }
    double Agility { get; }
    double HP_ratio { get; }
    double AP_ratio { get; }
    double ESS_ratio { get; }
    double Level { get; }

    double Evasion { get; set; }
    double Block { get; set; }

    double HP_max { get; set; }
    double HP_current { get; set; }
    string HP_status { get; set; }

    double AP_max { get; set; }
    double AP_current { get; set; }
    string AP_status { get; set; }

    double ESS_max { get; set; }
    double ESS_current { get; set; }
    string ESS_status { get; set; }

    double TN_current { get; set; }
    string TN_status { get; set; }

    List<AttackAction> attackList { get; set; }

    public void Act(int index);

}
