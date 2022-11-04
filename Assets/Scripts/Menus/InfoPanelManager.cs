using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanelManager : MonoBehaviour
{
    [SerializeField] GameObject infoEntryPrefab;
    [SerializeField] GameObject subHeaderPrefab;
    GameObject infoPanel;
    GameObject unitName;
    GameObject unitStats;
    GameObject unitBonuses;
    GameObject unitResistances;
    GameObject closeBtn;

    bool isInit = false;
    // Start is called before the first frame update
    void Init() {
        infoPanel = GameObject.FindGameObjectWithTag("Info");
        unitName = infoPanel.transform.Find("UnitName").gameObject;
        unitStats = infoPanel.transform.Find("Unit Stats").gameObject;
        unitBonuses = infoPanel.transform.Find("Unit Bonuses").gameObject;
        unitResistances = infoPanel.transform.Find("Unit Resistances").gameObject;

        closeBtn = infoPanel.transform.Find("Button").gameObject;
        closeBtn.GetComponent<Button>().onClick.AddListener(() => {
            infoPanel.SetActive(false);
            EmptyTable(unitStats);
            EmptyTable(unitBonuses);
            EmptyTable(unitResistances);
        });

        isInit = true;

    }
    void Awake()
    {
        if (!isInit) {
            Init();
        }
        
    }
    private void EmptyTable(GameObject _table) {
        foreach(Transform entry in _table.transform) {
            if (entry.gameObject.name != "Header") Destroy(entry.gameObject);
        }
    }

    public void SetName(Unit _unit) {
        if (!isInit) {
            Init();
        }
        unitName.GetComponent<Text>().text = _unit.name;
        PopulateStatsTable(_unit);
        PopulateTable(_unit, unitBonuses, "damageBonuses");
        PopulateTable(_unit, unitBonuses, "rateBonuses");
        PopulateTable(_unit, unitResistances, "damageResistances");
        PopulateTable(_unit, unitResistances, "ailmentResistances");
    }

    private void PopulateTable(Unit _unit, GameObject _table, string _attribute) {
        GameObject subHeader = Instantiate(subHeaderPrefab, _table.transform);
        subHeader.GetComponent<Text>().text = _attribute.SplitCamelCase();
        Debug.Log(_attribute);

        // Get the correct dictionary of bonuses/resistances
        PropertyInfo attributeList = typeof(UnitStats).GetProperty(_attribute);
        object dictionaryObj = attributeList.GetValue(_unit.unitStats, null);
        Dictionary<string, double> dictionary = (Dictionary<string, double>)(attributeList.GetValue(_unit.unitStats, null));
        
        List<string> keys = new List<string>(dictionary.Keys);
        foreach (string key in keys) {
            GameObject entry = Instantiate(infoEntryPrefab, _table.transform);
            entry.gameObject.name = key;

            GameObject title = entry.transform.Find("Title").gameObject;
            string str = key;
            if (key.Length > 1) {
                str = char.ToUpper(key[0]) + key.Substring(1);
            }
            title.GetComponent<Text>().text = str;

            GameObject value = entry.transform.Find("Value").gameObject;
            string valueString = dictionary[key].ToString();
            value.GetComponent<Text>().text = valueString;
        }

    }
    private void PopulateStatsTable(Unit _unit) {
        if (!isInit) {
            Init();
        }
        List<string> keys = new List<string>(_unit.unitStats.statBonuses.Keys);
        foreach (string key in keys) {
            GameObject entry = Instantiate(infoEntryPrefab, unitStats.transform);
            entry.gameObject.name = key;
            GameObject title = entry.transform.Find("Title").gameObject;
            string str = key;
            if (key.Length > 1) {
                str = char.ToUpper(key[0]) + key.Substring(1);
            }
            title.GetComponent<Text>().text = str;
            GameObject value = entry.transform.Find("Value").gameObject;
            PropertyInfo prop = typeof(UnitStats).GetProperty(key);
            Debug.Log(key + ": " + _unit.unitStats.statBonuses[key]);
            value.GetComponent<Text>().text = (prop.GetValue(_unit.unitStats, null)).ToString() + " (+" + _unit.unitStats.statBonuses[key].ToString() + ")";
        }
    }
}
