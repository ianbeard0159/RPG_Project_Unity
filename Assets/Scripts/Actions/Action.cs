using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action : MonoBehaviour
{
    public Unit owner { get; set; }
    public double AP_cost;
    public double ESS_cost;
    public double numTargets;
}
