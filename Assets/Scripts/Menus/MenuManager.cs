using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    // Text Prefabs
    [SerializeField] private GameObject myMenuItem;
    [SerializeField] private GameObject myMenuItemInactive;

    // Current Unit
    private CharacterUnit currentUnit;
    private int unitIndex;

    // Menu container
    private GameObject menuBox;
    int xPos;
    int yPos;
    int yDisplace;

    // Menus
    private GameObject actionMenu;
    private GameObject actionSubMenu;
    private GameObject targetMenu;
    private GameObject infoPanel;
    private InfoPanelManager infoPanelManager;

    // Initialization flag
    private bool isInit = false;

    // Initialize Menus
    private void Init() {
        // Identify the menus
        menuBox = GameObject.Find("MenuBox");
        actionMenu = menuBox.transform.Find("ActionMenu").gameObject;
        actionSubMenu = menuBox.transform.Find("ActionSubMenu").gameObject;
        targetMenu = menuBox.transform.Find("TargetMenu").gameObject;

        infoPanel = GameObject.FindWithTag("Info");
        infoPanelManager = infoPanel.GetComponent<InfoPanelManager>();
        // Set all menus as inactive
        actionMenu.SetActive(false);
        actionSubMenu.SetActive(false);
        targetMenu.SetActive(false);
        infoPanel.SetActive(false);
        // Set menu anchor point
        xPos = 0;
        yPos = 80;
        yDisplace = -35;
        // Tell the program that the menus have been initialized
        isInit = true;
    }

    private void ClearMenu(GameObject menu) {
        foreach (Transform entry in menu.transform)
        {
            Destroy(entry.gameObject);
        }
    }

    private void AddBackBtn(GameObject currentMenu, GameObject prevMenu, int posIndex) {
        // Create an instance of the back button within the current menu
        GameObject backBtn = Instantiate(myMenuItem, currentMenu.transform);
        backBtn.name = "Back";
        backBtn.GetComponent<Text>().text = "Back";
        backBtn.GetComponent<Button>().onClick.AddListener(() => {
            ClearMenu(currentMenu);
            currentMenu.SetActive(false);
            prevMenu.SetActive(true);
        });
        backBtn.transform.localPosition = new Vector2(xPos, (yDisplace*posIndex + yPos));
    }

    private void AddActionBtn(string _name, List<Action> actionList, Type type, int posIndex) {
        GameObject btn;
        btn = Instantiate(myMenuItem, actionMenu.transform);
        btn.name = _name;
        btn.GetComponent<Text>().text = _name;
        btn.GetComponent<Button>().onClick.AddListener(() => {
            // Create new instance of enemy list
            List<Unit> potentialTargets = new List<Unit>();
            if (type == typeof(AttackAction)) {
                foreach (Unit enemy in GameDriver.Instance.enemyList) {
                    potentialTargets.Add(enemy);
                }
            }
            if (type == typeof(SupportAction)) {
                foreach (Unit character in GameDriver.Instance.characterList) {
                    potentialTargets.Add(character);
                }
            }
            PopulateActionSubMenu(actionList, potentialTargets, type);
            actionMenu.SetActive(false);
        });
        btn.transform.localPosition = new Vector2(xPos, (yDisplace*posIndex + yPos));
    }

    //
    // --=|| Target Menu ||=--
    //
    private void PopulateTargetMenu(Action action, List<Unit> potentialTargets, Type type) {
        // Initialize Menus
        if (!isInit) Init();
        // Reset the targets menu
        ClearMenu(targetMenu);
        targetMenu.SetActive(true);


        int posIndex = 0;
        List<Unit> targets = new List<Unit>();
        string btnText = "";
        // If the selected action effects all available targets,
        //    make one "All Targets" button
        if (action.numTargets == -1) {
            GameObject allTargetsBtn = Instantiate(myMenuItem, targetMenu.transform);
            // If the selected action is an attack
            if (type == typeof(AttackAction)) {
                btnText = "All Enemies";
            }
            // If the selected action is a support ability
            else if (type == typeof(SupportAction)) {
                btnText = "All Allies";
            }
            // If the action type has not been implemented yet
            else {
                Debug.Log("Check");
                PopulateActionMenu(currentUnit, unitIndex);
                targetMenu.SetActive(false);
            }
            allTargetsBtn.name = btnText;
            allTargetsBtn.GetComponent<Text>().text = btnText;
            allTargetsBtn.GetComponent<Button>().onClick.AddListener(() => {
                GameDriver.Instance.PerformAction(potentialTargets, currentUnit, action);
                PopulateActionMenu(currentUnit, unitIndex);
                targetMenu.SetActive(false);
            });
            allTargetsBtn.transform.localPosition = new Vector2(xPos, yPos);
            posIndex += 1;
        }

        // If the action effects a given number of targets,
        //     let the player select from a list of targets
        else if (action.numTargets > 1) {
            // TODO implement multi target menu
        }

        // If the selected action only effects one target,
        //     let the player select from a list of targets
        else if (action.numTargets == 1) {
            // Add all enemies to the target menu
            foreach (Unit target in potentialTargets) {
                // Initialize menu item
                GameObject btn = Instantiate(myMenuItem, targetMenu.transform);
                // Set the properties of the menu item
                btn.name = target.gameObject.name;
                btn.GetComponent<Text>().text = target.gameObject.name;
                btn.GetComponent<Button>().onClick.AddListener(() => {
                    // When selected, deal damage to the selected enemy, 
                    //    then reset to the action menu
                    GameDriver.Instance.PerformAction(target, currentUnit, action);
                    PopulateActionMenu(currentUnit, unitIndex);
                    targetMenu.SetActive(false);
                });
                btn.transform.localPosition = new Vector2(xPos, (yDisplace*posIndex + yPos));
                posIndex += 1;
            }
        }

        // If the selected action effects the user
        else if (action.numTargets == 0) {
            Debug.Log("Implement self target");
        }

        // Add a back button that resets to the attacks menu
        AddBackBtn(targetMenu, actionSubMenu, posIndex);
    }

    //
    // --=|| Action Sub Menu ||=--
    //
    private void PopulateActionSubMenu(List<Action> actionList, List<Unit> potentialTargets, Type type) {
        // Set Action Sub Menu as the active menu
        actionMenu.SetActive(false);
        actionSubMenu.SetActive(true);

        // Reset the Action Sub Menu
        ClearMenu(actionSubMenu);

        // Add each action from the input action list
        int posIndex = 0;
        foreach (Action action in actionList) {
            // Create an instance of the action button within the action sub menu
            GameObject btn = Instantiate(myMenuItem, actionSubMenu.transform);
            btn.name = action.gameObject.name;
            btn.GetComponent<Text>().text = action.gameObject.name;
            btn.GetComponent<Button>().onClick.AddListener(() => {
                // When selected, move to the targets menu using the
                //    selected support
                PopulateTargetMenu(action, potentialTargets, type);
                actionSubMenu.SetActive(false);
            });
            btn.transform.localPosition = new Vector2(xPos, (yDisplace*posIndex + yPos));
            posIndex += 1;
        }
        // Add back button to the list of menu items
        AddBackBtn(actionSubMenu, actionMenu, posIndex);
    }

    //
    // --=|| Action Menu ||=--
    //
    public void PopulateActionMenu(CharacterUnit character, int index) {
        if (!GameDriver.Instance.gameOverFlag) {
            // Make sure that the current unit can be accessed by all methods
            currentUnit = character;
            unitIndex = index;

            // Initialize the menus if necessary
            if (!isInit) Init();

            // Reset the actions menu
            ClearMenu(actionMenu);
            actionMenu.SetActive(true);


            //
            // --=|| Attack Option ||=--
            //
            int posIndex = 0;
            // If the character is allowed to perform attacks
            if (character.attackList.Count > 0) {
                AddActionBtn("Attack", character.attackList, typeof(AttackAction), posIndex);
            }
            // May include other conditions for this in the future
            else {
                // Initialize inactive attack option
                GameObject attackOption = Instantiate(myMenuItemInactive, actionMenu.transform);
                attackOption.GetComponent<Text>().text = "Attack";
                attackOption.transform.localPosition = new Vector2(xPos, yPos);
            }
            posIndex += 1;

            //
            // --=|| Support Option ||=--
            //
            AddActionBtn("Support", character.supportList, typeof(SupportAction), posIndex);
            posIndex += 1;
            
            //
            // --=|| Info Option ||=--
            //
            GameObject infoOption = Instantiate(myMenuItem, actionMenu.transform);
            infoOption.name = "Info";
            infoOption.GetComponent<Text>().text = "Info";
            infoOption.GetComponent<Button>().onClick.AddListener(() => {
                infoPanel.SetActive(true);
                infoPanelManager.SetName((Unit)currentUnit);
            });
            infoOption.transform.localPosition = new Vector2(xPos, (yDisplace*posIndex + yPos));
            posIndex += 1;

            //
            // --=|| End turn option ||=--
            //
            GameObject endTurnOption = Instantiate(myMenuItem, actionMenu.transform);
            endTurnOption.name = "EndTurn";
            endTurnOption.GetComponent<Text>().text = "End Turn";
            endTurnOption.GetComponent<Button>().onClick.AddListener(() => {
                // When clicked, deactivate the menu and start
                //    the next unit's turn
                actionMenu.SetActive(false);
                GameDriver.Instance.NextTurn(index + 1);
            });
            endTurnOption.transform.localPosition = new Vector2(xPos, (yDisplace*posIndex + yPos));
        }
        else {
            // If the game is over, deactivate all menus
            actionMenu.SetActive(false);
            actionSubMenu.SetActive(false);
            targetMenu.SetActive(false);

        }
    }
}
