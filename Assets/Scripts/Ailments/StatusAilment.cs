using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusAilment : MonoBehaviour, IEquatable<StatusAilment>
{
    public string category;
    public string type;
    public MinorAilment minor;
    public MajorAilment major;
    public double buildup;
    public bool minorApplied {get; set;}
    public bool majorApplied {get; set;}

    
    public bool Equals(StatusAilment _ailment) {
        if (_ailment == null) {
            return false;
        }

        if (_ailment.type == this.type) {
            return true;
        }
        else {
            return false;
        }
    }
    public void AddAilment(Unit _unit, double _buildup) {
        if (!_unit.ailmentList.Contains(this)) {
            StatusAilment ailmentInstance = Instantiate(this, _unit.gameObject.transform);
            _unit.ailmentList.Add(ailmentInstance);
            int index = _unit.ailmentList.IndexOf(ailmentInstance);
            _unit.ailmentList[index].ChangeBuildup(_unit, _buildup);
        }
        else {
            int index = _unit.ailmentList.IndexOf(this);
            _unit.ailmentList[index].ChangeBuildup(_unit, _buildup);
        }
    }
    
    public void CheckAilment(Unit _unit) {
        // Apply a major version of the ailment at 100% buildup
        if (buildup >= 100) {
            majorApplied = true;
            major.ApplyAilment(_unit);
        }
        else if (buildup >= 50) {
            // re-apply a major version ailment if it is a persistent ailment
            if (major.persist && majorApplied) {
                major.ApplyAilment(_unit);
            }
            else if (majorApplied) {
                major.RemoveAilment(_unit);
                majorApplied = false;
            }

            // At 50% buildup, apply a minor version of the ailment if
            //    the ailment has not already been applied
            if (!majorApplied && (!minorApplied || minor.applyOnHit)){
                // The "minorApplied" flag is ignored if the ailment is set to be
                //    applied every time it is inflicted after 50% buildup
                minorApplied = true;
                minor.ApplyAilment(_unit);
            }
        }
        // Ensure the ailment is not applied below 50% buildup
        else if (buildup > 0) {
            minor.RemoveAilment(_unit);
            major.RemoveAilment(_unit);
            majorApplied = false;
            minorApplied = false;
        }
        else {
            int index = _unit.ailmentList.IndexOf(this);
            Destroy(_unit.ailmentList[index].gameObject);
            _unit.ailmentList.RemoveAt(index);
        }
    }

    public void ChangeBuildup(Unit _unit, double _change) {
        buildup = (buildup + _change).EnforceRange(100, 0);
        // Dont check automatically if the buildup went down
        if (_change > 0) CheckAilment(_unit);
    }

    public void RefreshAilment(Unit _unit) {
        majorApplied = false;
        minorApplied = false;
        // Apply on hit effects shouldn't also be applied every turn
        if (!minor.applyOnHit || buildup >= 100) CheckAilment(_unit);
        ChangeBuildup(_unit, -10);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
