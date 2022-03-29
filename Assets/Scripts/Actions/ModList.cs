using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModList : MonoBehaviour
{
    public List<Modifier> modifiers;

    public void ApplyModifiers(Unit unit) {
        List<Modifier> initialMods = new List<Modifier>(unit.modifierList);
        foreach (Modifier mod in modifiers) {
            // If the modifier is not already applied, apply it
            Modifier tempMod = Instantiate(mod, unit.gameObject.transform);
            tempMod.name = tempMod.name.Replace("(Clone)", "");
            if (!unit.modifierList.Contains(tempMod)){
                unit.modifierList.Add(tempMod);
            }
            // If the modifier is applied, reset it's duration
            else {
                int modIndex = unit.modifierList.IndexOf(tempMod);
                unit.modifierList[modIndex].currentDuration = mod.duration;
                Destroy(tempMod.gameObject);
            }
        }
        unit.unitController.UpdateModifiers(initialMods);
    }
}
