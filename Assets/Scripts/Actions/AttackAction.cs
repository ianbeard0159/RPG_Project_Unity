using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : Action
{
    public AttackController controller;

    public string attackType;
    public double baseAccuracy;
    public double critChance;
    public double baseDamage;
    public double numHits;
    public double baseAggro;

    // Start is called before the first frame update
    void Awake()
    {
        controller = new AttackController(this);
    }

}
