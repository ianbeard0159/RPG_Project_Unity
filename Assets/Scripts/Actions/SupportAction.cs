using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportAction : Action
{
    public double healRatio;
    public ModList modifiers { get; private set; }

    private void Awake() {
        modifiers = gameObject.GetComponent<ModList>();
    }
}
