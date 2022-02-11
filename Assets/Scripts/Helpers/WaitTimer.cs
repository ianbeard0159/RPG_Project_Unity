using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTimer : MonoBehaviour
{
    public void WaitThenNextTurn(float timeInterval, int index)
    {
        StartCoroutine(WaitThenNextTurnRoutine(timeInterval, index));
    }
    private IEnumerator WaitThenNextTurnRoutine(float seconds, int index)
    {
        yield return new WaitForSeconds(seconds);
        GameDriver.Instance.NextTurn(index);
    }
}
