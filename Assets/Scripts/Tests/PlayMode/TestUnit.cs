using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class TestUnit
{
    [TestFixture] public class SpendResources_Should {    
        public CharacterUnit jason;
        [SetUp] public void Setup() {
            GameObject jasonGameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Units/Characters/Jason"));
            jasonGameObject.name = jasonGameObject.name.Replace("(Clone)", "");
            jason = jasonGameObject.GetComponent<CharacterUnit>();
        }

        [UnityTest] public IEnumerator ChangeUnitResources() {
            // Arange
            double ESS_amount = 100;
            double AP_amount = 2;

            double ESS_initial = jason.ESS_current;
            double AP_initial = jason.AP_current;
            // Act
            double essenceBurn = jason.SpendResources(AP_amount, ESS_amount);
            // Assert
            yield return null;
            if (essenceBurn == 0) {
                Assert.AreEqual(jason.ESS_current, ESS_initial - ESS_amount);
                Debug.Log(ESS_initial + " - " + ESS_amount + " = " + jason.ESS_current);
            }
            else {
                Assert.AreEqual(jason.ESS_current, 0);
            }
        }

        [TearDown] public void Teardown() {
            UnityEngine.Object.Destroy(jason.gameObject);
        }
    }

    [TestFixture] public class TakeDamage_Should {
        private CharacterUnit jason;
        private DamageDealt damageData = new DamageDealt();
        private DamageTaken takenData = new DamageTaken();
        private double HP_initial;
        private double TN_initial;
        private double TN_potential;

        [SetUp] public void TakeDamage_Setup() {
            // Mock damage data
            damageData.damage = 500;
            damageData.type = "electric";
            damageData.crit = 1.0;
            damageData.aggro = 5;
            damageData.ignoreBlock = false;
            damageData.ignoreCounter = false;
            damageData.ignoreEvasion = false;
            
            // Test unit
            GameObject jasonGameObject = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Prefabs/Units/Characters/Jason"));
            jasonGameObject.name = jasonGameObject.name.Replace("(Clone)", "");
            jason = jasonGameObject.GetComponent<CharacterUnit>();

            // Initial resources of test unit
            HP_initial = jason.HP_current;
            TN_initial = jason.TN_current;

            // Run TakeDamage()
            takenData = jason.TakeDamage(damageData);
            TN_potential = Math.Ceiling(((damageData.damage / jason.HP_max) / 10) * 100) / 100;


        }
        [UnityTest] public IEnumerator HaveValidResult() {
            string[] expectedResultString = new string[] {"miss", "evaded", "blocked", "partially blocked", "taken"};

            yield return null;
            Assert.Contains(takenData.result, expectedResultString);
            Debug.Log(takenData.result);
        }
        [UnityTest] public IEnumerator DealDamageIfHit() {
            yield return null;
            Debug.Log(takenData.result);
            if(takenData.result != "partially blocked" && takenData.result != "taken") {
                Assert.AreEqual(jason.HP_current, HP_initial);
            }
            else {
                Assert.AreNotEqual(jason.HP_current, HP_initial);
            }
        }
        [UnityTest] public IEnumerator ChangeTension() {
            yield return null;
            Debug.Log(takenData.result + " | " + TN_potential);
            Debug.Log(jason.TN_current + " = ");
            if(takenData.result == "evaded" || takenData.result == "blocked") {
                Assert.AreEqual(jason.TN_current, TN_initial + TN_potential);
                Debug.Log(TN_initial + " + " + TN_potential);
            }
            else if (takenData.result == "partially blocked") {
                Assert.AreEqual(jason.TN_current, TN_initial - (TN_potential / 2));
                Debug.Log(TN_initial + " - " + TN_potential + "/2");
            }
            else {
                Assert.AreEqual(jason.TN_current, TN_initial - TN_potential);
                Debug.Log(TN_initial + " - " + TN_potential);
            }
        }

        [UnityTest] public IEnumerator ApplyModifier() {
            yield return null;
        }
        [UnityTest] public IEnumerator ApplyAilment() {
            yield return null;
        }

        [TearDown] public void Teardown() {
            UnityEngine.Object.Destroy(jason.gameObject);
        }

    }
}
