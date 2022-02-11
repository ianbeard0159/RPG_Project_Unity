using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUnit : Unit
{

    void Awake()
    {
        unitController = new UnitController(this);
        unitController.Init();
    }
}
