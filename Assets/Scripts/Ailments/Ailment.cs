using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ailment : MonoBehaviour
{
    [SerializeField] private double _HP_loss;
    public double HP_loss { 
        get { return _HP_loss; } 
        set { _HP_loss = value; }
    }
    [SerializeField] private double _AP_loss;
    public double AP_loss { 
        get { return _AP_loss; } 
        set { _AP_loss = value; }
    }
    [SerializeField] private double _ESS_loss;
    public double ESS_loss { 
        get { return _ESS_loss; } 
        set { _ESS_loss = value; }
    }
    [SerializeField] private double _TN_loss;
    public double TN_loss { 
        get { return _TN_loss; } 
        set { _TN_loss = value; }
    }

    public ModList modList {get; set;}


    public void ApplyAilment(Unit _unit) {
        Debug.Log(_unit.name);
        _unit.unitController.ChangeHealth(-HP_loss);
        _unit.unitController.ChangeTN(-TN_loss);
        _unit.unitController.ChangeESS(-ESS_loss);
        _unit.unitController.ChangeAP(-AP_loss);

        gameObject.GetComponent<ModList>().ApplyModifiers(_unit);

    }
    public void RemoveAilment(Unit _unit) {
        foreach (Modifier mod in gameObject.GetComponent<ModList>().modifiers) {
            Debug.Log(mod.name);
            Debug.Log(_unit.modifierList.Contains(mod));
            if (_unit.modifierList.Contains(mod)) {
                int index = _unit.modifierList.IndexOf(mod);
                Transform modObject = _unit.gameObject.transform.Find(mod.name);
                Destroy(modObject.gameObject);
                _unit.modifierList.RemoveAt(index);
            }
        }
    }


    // Start is called before the first frame update
    void Awake()
    {
        modList = gameObject.GetComponent<ModList>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
