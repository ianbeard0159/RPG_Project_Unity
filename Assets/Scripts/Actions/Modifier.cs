using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier : MonoBehaviour, IEquatable<Modifier>
{
    public string type;
    public string attribute;
    public double value;
    public double duration;
    public double currentDuration;

    private void Awake() {
        currentDuration = duration;
    }

    public bool Equals(Modifier _mod) {
        if (_mod == null) {
            return false;
        }

        if (_mod.name == this.name) {
            return true;
        }
        else {
            return false;
        }
    }

    public void SelfDestruct() {
        Destroy(this.gameObject);
    }


}
