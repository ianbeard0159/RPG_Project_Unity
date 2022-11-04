using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum attResult {
    miss,
    evaded,
    blocked,
    partially_blocked,
    taken
}

public enum AilmentType {
    mundane,
    physical,
    elemental
}

public enum PhysicalAilment {
    stagger,
    stun,
    bleed,
    poison,
    fatigue
}
public enum MentalAilment {
    fear,
    anger,
    confusion,
    despair,
    pressure
}
public enum ElementalAilment {
    burn, 
    harden,
    suffocate,
    shock,
    chill
}
