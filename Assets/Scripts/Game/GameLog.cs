using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLog : MonoBehaviour
{
    [SerializeField] private GameObject characterStartText;
    [SerializeField] private GameObject characterEventText;
    [SerializeField] private GameObject enemyStartText;
    [SerializeField] private GameObject enemyEventText;

    private GameObject logText;
    private List<Transform> logTextList;
    void Awake() {
        logTextList = new List<Transform>();
    }
    private void ShiftOldText() {
        logTextList.Clear();
        foreach (Transform entry in GameDriver.Instance.gameLogBox.transform) {
            logTextList.Add(entry);
        }
        logTextList.Reverse();
        int index = 1;
        foreach (Transform entry in logTextList) {
            if (index >= 4) {
                Destroy(entry.gameObject);
            }
            entry.name = "entry " + (index + 1);
            entry.transform.localPosition = new Vector2(10, 40*index - 60);
            index++;
        }
    }
    public void DisplayText(string logString, string logType) {
        ShiftOldText();
        if (logType == "characterEvent") logText = characterEventText;
        else if (logType == "enemyEvent") logText = characterEventText;
        else if (logType == "characterStart") logText = characterStartText;
        
        GameObject log = Instantiate(logText, GameDriver.Instance.gameLogBox.transform);
        log.name = "entry 1";
        log.GetComponent<Text>().text = logString;
        log.transform.localPosition = new Vector2(10, -60);
        
    }
    // Waiting Functions
    public void WaitThenDisplay(float seconds, string logString, string logType) {
        StartCoroutine(WaitThenDisplayRoutine(seconds, logString, logType));
    }
    private IEnumerator WaitThenDisplayRoutine(float seconds, string logString, string logType) {
        yield return new WaitForSeconds(seconds);
        DisplayText(logString, logType);
    }
}
