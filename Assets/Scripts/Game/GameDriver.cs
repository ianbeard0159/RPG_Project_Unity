using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameDriver : MonoBehaviour
{
    public static GameDriver Instance;
    // Lists
    private List<Unit> turnOrder = new List<Unit>();
    public List<Unit> enemyList = new List<Unit>();
    public List<Unit> characterList = new List<Unit>();
    public List<ActionTargetEntry> targetRegister = new List<ActionTargetEntry>();

    // Menus
    private MenuManager menuManager;


    // Timer
    private WaitTimer timer;

    // Game Log Window
    public GameObject gameLogBox;
    public GameLog gameLog;

    public bool gameOverFlag { get; private set; }
    
    private void Start() {
        // Create a static instance of the gamedriver for other classes to use
        if (!Instance) {
            Instance = this;
        }
        // Get the menus that the gamedriver will use
        gameLogBox = GameObject.Find("GameLog");
        gameLog = gameLogBox.GetComponent<GameLog>();
        menuManager = GameObject.Find("MenuBox").GetComponent<MenuManager>();
        timer = GameObject.Find("Battlefield").GetComponent<WaitTimer>();
        // Set a flag for ending the battle
        gameOverFlag = false;

        InitializeLists();
        NextTurn(0);
    }

    public void InitializeLists()
    {
        // Add all characters to the turn order list
        GameObject[] playerUnits = GameObject.FindGameObjectsWithTag("CharacterUnit");
        foreach (GameObject playerUnit in playerUnits) {
            CharacterUnit currentUnit = playerUnit.GetComponent<CharacterUnit>();
            turnOrder.Add(currentUnit);
            characterList.Add(currentUnit);
        }

        // Add enemy unit
        GameObject[] enemyUnits = GameObject.FindGameObjectsWithTag("EnemyUnit");
        foreach (GameObject enemyUnit in enemyUnits) {
            EnemyUnit currentUnit = enemyUnit.GetComponent<EnemyUnit>();
            currentUnit.AI_instance.enemyAttackPriority.PopulateAggroTable(characterList);
            turnOrder.Add(currentUnit);
            enemyList.Add(currentUnit);

        }
        // Sort based on agility
        turnOrder.Sort();
        turnOrder.Reverse();
    }

    public void NextTurn(int index) {
        if (!gameOverFlag) {
            // Loop back to the start of the list if necessary
            if (index >= turnOrder.Count) {
                SupportPriority.CheckDuration();
                index = 0;
            }
            // Take the current unit's turn unless they have 0 health
            Unit currentUnit = turnOrder[index];
            // Take player turn
            if (currentUnit.gameObject.tag == "CharacterUnit") {
                if (currentUnit.HP_status != "incapacitated") {
                    // Start the character's turn and populate the action menu
                    string logString = "Player Turn: "  + currentUnit.name;
                    gameLog.DisplayText(logString, "characterStart");
                    currentUnit.unitController.StartTurn();
                    menuManager.PopulateActionMenu((CharacterUnit)currentUnit, index);
                }
                else {
                    // If incapacitated, skip the character's turn
                    string logString = currentUnit.name + " is Incapacitated";
                    gameLog.DisplayText(logString, "characterStart");
                    NextTurn(index + 1);
                }
            }
            // Take enemy turn
            else {
                if (currentUnit.HP_status != "incapacitated") 
                {
                    // Take enemy turn, then wait before starting the next turn
                    string logString = "Enemy Turn: "  + currentUnit.name;
                    gameLog.WaitThenDisplay(0.2f, logString, "characterStart");
                    int timeInterval = ((EnemyUnit)currentUnit).enemyController.TakeEnemyTurn();
                    timer.WaitThenNextTurn(timeInterval, index + 1);
                }
                else 
                {
                    // If incapacitated, remove the enemy from the battle
                    turnOrder.Remove(currentUnit);
                    Destroy(currentUnit.gameObject);
                    NextTurn(index);
                }
            }
        }
    }

    public void SelfDestruct(Unit _unit) {
        turnOrder.Remove(_unit);
        enemyList.Remove((EnemyUnit)_unit);
        Destroy(_unit.gameObject);
    }

    // Multi Target
    public float PerformAction(List<Unit> _targets, Unit _owner, Action _action) {
        float timeInterval = 1.5f;

        if(CheckResources(_owner, _action, "")) {
            if (_action.GetType() == typeof(AttackAction)) {
                foreach (Unit target in _targets) {
                    timeInterval = PerformAttack(target, _owner, (AttackAction)_action);
                }
            }
            else if (_action.GetType() == typeof(SupportAction)) {
                foreach (Unit target in _targets) {
                    timeInterval = PerformSupport(target, _owner, (SupportAction)_action);
                }
            }

            CheckWinLose(timeInterval);

            return timeInterval;
        }
        else {
            return 1f;
        }
    }
    // Single Target
    public float PerformAction(Unit _target, Unit _owner, Action _action) {
        float timeInterval = 1.5f;
        string targetStr = " on " + _target.name;
        if(CheckResources(_owner, _action, targetStr)) {
            if (_action.GetType() == typeof(AttackAction)) {
                timeInterval = PerformAttack(_target, _owner, (AttackAction)_action);   
            }
            else if (_action.GetType() == typeof(SupportAction)) {
                timeInterval = PerformSupport(_target, _owner, (SupportAction)_action);
            }
            CheckWinLose(timeInterval);  
            return timeInterval;
        }
        else {
            return 1f;
        }
    }

    private bool CheckResources(Unit _owner, Action _action, string _targetStr) {
        string logString = "";

        logString = _owner.gameObject.name + " used " + _action.name + _targetStr;
        gameLog.WaitThenDisplay(0.5f, logString, "characterEvent");

        // Don't perform the atack if the unit does not have enough AP
        if (_action.AP_cost > _owner.AP_current) {
            logString = "But " + _owner.gameObject.name + " didn't have enough AP ";
            gameLog.WaitThenDisplay(1f, logString, "characterEvent");

            return false;
        }

        // Spend AP and ESS, and make the unit take damage
        //    if they spent more ESS than they had
        double essBurn = _owner.unitController.SpendResources(_action.AP_cost, _action.ESS_cost);
        if (essBurn != 0) {
            logString = _owner.gameObject.name + " didn't have enough Essence, and took " + -essBurn + " of essence burn";
            gameLog.WaitThenDisplay(1f, logString, "characterEvent");

        }
        return true;

    }

    public float PerformAttack(Unit _target, Unit _attacker, AttackAction _attack)
    {
        string logString = "";
        float timeInterval = 1.5f;

        // Seal damage to the target
        for (float i = 0; i < _attack.numHits; i++)
        {
            DamageDealt damageData = _attack.controller.DealDamage(_target);
            DamageTaken takenData = _target.unitController.TakeDamage(damageData);

            EnemyUnit enemy;
            CharacterUnit character;
            double aggroChange;
            if(_target.GetType() == typeof(EnemyUnit)) {
                enemy = (EnemyUnit)_target;
                character = (CharacterUnit)_attacker;
                aggroChange = _attack.baseAggro;
            }
            else {
                enemy = (EnemyUnit)_attacker;
                character = (CharacterUnit)_target;
                aggroChange = -_attack.baseAggro;
            }

            switch (takenData.result)
            {
                case "miss":
                    logString = "But the attack missed.";
                    break;
                case "blocked":
                    logString = "but " + _target.gameObject.name + " blocked the attack.";
                    break;
                case "evaded":
                    logString = "but " + _target.gameObject.name + " evaded the attack.";
                    break;
                case "partially blocked":
                    if (damageData.crit != 1) {
                        logString = "Critical Hit! ";
                        _attacker.unitController.ChangeTN(0.02);
                        aggroChange = Math.Round((aggroChange/2) * 1.5);
                    }
                    else logString = "";

                    logString += _target.gameObject.name + " partially blocked, taking " + takenData.damage + " damage.";
                        UpdateEnemyAggro(enemy, character, aggroChange);
                    break;
                case "taken":
                    if (damageData.crit != 1) {
                        logString = "Critical Hit! ";
                        _attacker.unitController.ChangeTN(0.03);
                        aggroChange = Math.Round(aggroChange * 1.5);
                    }
                    else logString = "";

                    logString += _target.gameObject.name + " took " + takenData.damage + " damage.";
                    UpdateEnemyAggro(enemy, character, aggroChange);
                    break;
            }
            // Add to the time that the game log will wait to display information
            timeInterval += i / 2f;
            gameLog.WaitThenDisplay(timeInterval, logString, "characterEvent");

        }
        timeInterval += 0.5f;

        return timeInterval;

    }

    public float PerformSupport(Unit target, Unit caster, SupportAction _support) {
        float timeInterval = 1.5f;

        // Heal if heal ratio isn't zero

        // If there are modifiers, apply them to the target
        _support.modifiers.ApplyModifiers(target);

        return timeInterval;
    }
    private void UpdateEnemyAggro(EnemyUnit enemy, CharacterUnit character, double aggro) {
        enemy.AI_instance.enemyAttackPriority.UpdateAggroTable(character, aggro);

    }

    private void CheckWinLose(float timeInterval) {
        // The player wins if there are no enemies left
        if (enemyList.Count == 0) {
            gameOverFlag = true;
            gameLog.WaitThenDisplay(timeInterval,"You Win!", "characterStart");
        }
        // The player loses if all characters are incapacitated
        else {
            gameOverFlag = true;
            foreach (CharacterUnit character in characterList) {
                if (character.HP_status != "incapacitated") gameOverFlag = false;
            }
            if (gameOverFlag) {
                gameLog.WaitThenDisplay(timeInterval,"You Lose!", "characterStart");
            }
        }
    }
}
