using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggroTableEntry : IComparable {
    public CharacterUnit character;
    public double aggro;
    public AggroTableEntry(CharacterUnit inCharacter) {
        character = inCharacter;
        aggro = 0;
    }
    public int CompareTo(object otherEntry) {
        return aggro.CompareTo(((AggroTableEntry)otherEntry).aggro);
    }
}
