﻿/*****************************************
 * This file is part of Impulse Framework.

    Impulse Framework is free software: you can redistribute it and/or modify
    it under the terms of the GNU Lesser General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    Impulse Framework is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Lesser General Public License for more details.

    You should have received a copy of the GNU Lesser General Public License
    along with Impulse Framework.  If not, see <http://www.gnu.org/licenses/>.
*****************************************/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Generic Detector Script (Detects Generic Faction Objects)
/// </summary>
public class Detector : MonoBehaviour
{

    // Detection parameters
    [SerializeField]
    [Range(0, 1000)]
    float detectionRange = 15.0f;
    [SerializeField]
    [Range(0, 1000)]
    float attackRange = 5.0f;
    [SerializeField]
    [Range(0, 1000)]
    float avoidRange = 1.0f;
    [SerializeField]
    [Range(0, 5)]
    float detectionRate = 0.1f;
    [SerializeField]
    float largestRange = 0.0f;

    // Faction parameters
    [SerializeField]
    List<Faction.Factions> AllyFactions = new List<Faction.Factions>();
    [SerializeField]
    List<Faction.Factions> EnemyFactions = new List<Faction.Factions>();

    // Detected Objects
    public List<Faction> DetectedEnemies = new List<Faction>();
    public List<Faction> DetectedAllies = new List<Faction>();
    public List<Faction> DetectedNeutral = new List<Faction>();

    // Debug
    [SerializeField]
    bool DisplayDetectionRange = true;
    [SerializeField]
    bool DisplayAttackRange = true;
    [SerializeField]
    bool DisplayAvoidRange = true;

    #region Gameplay

    // Use this for initialization
    void Start()
    {
        // Get largest search range
        float[] ranges = new float[] { detectionRange, attackRange, avoidRange };
        largestRange = Mathf.Max(ranges);

        // Start Automatic Faction Detection
        InvokeRepeating("DetectFactionObjects", 0.0f, detectionRate);
    }

    /// <summary>
    /// Detects Faction Objects in Range
    /// </summary>
    private void DetectFactionObjects()
    {
        // Clear previous detected characters
        DetectedEnemies.Clear();
        DetectedAllies.Clear();
        DetectedNeutral.Clear();

        // Detect all characters in range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, largestRange);
        int i = 0;
        while (i < hitColliders.Length)
        {
            //Ignore self object
            if (hitColliders[i].gameObject == gameObject)
			{
	            // Move to the next object
				i++;
                continue;
			}

            //Check if the object has a faction
            Faction currentObject = hitColliders[i].GetComponent<Faction>();

            //Organize the detected factions
            if (currentObject != null)
            {
				// Check if it is an ally
                if (AllyFactions.Contains(currentObject.FactionName))
                {
                    DetectedAllies.Add(currentObject);
                }
                // Check if it is an enemy
                else if (EnemyFactions.Contains(currentObject.FactionName))
                {
                    DetectedEnemies.Add(currentObject);
                }
                //Otherwise, set it as a neutral object
                else
                    DetectedNeutral.Add(currentObject);
            }

            // Move to the next object
            i++;
        }
    }
    #endregion

    #region Utilities
    /// <summary>
    /// Returns closest enemy game object within detection range.
    /// </summary>
    /// <returns></returns>
    public GameObject ClosestEnemy()
    {
        GameObject closestEnemy = null;
        float currentMinDistance = Mathf.Infinity;

        // Analize all detected enemies
        foreach (Faction current in DetectedEnemies)
        {
            // Get distance to the enemy
            float distance = Vector3.Distance(transform.position, current.transform.position);

            // First check the enemy is in the detection range and is the closest distance found until now
            if (distance <= detectionRange && distance < currentMinDistance)
                closestEnemy = current.gameObject;
        }

        return closestEnemy;
    }

    /// <summary>
    /// Returns closest ally game object within detection range.
    /// </summary>
    /// <returns></returns>
    public GameObject ClosestAlly()
    {
        GameObject closestAlly = null;
        float currentMinDistance = Mathf.Infinity;

        // Analize all detected enemies
        foreach (Faction current in DetectedAllies)
        {
            // Get distance to the enemy
            float distance = Vector3.Distance(transform.position, current.transform.position);

            // First check the enemy is in the detection range and is the closest distance found until now
            if (distance <= detectionRange && distance < currentMinDistance)
                closestAlly = current.gameObject;
        }

        return closestAlly;
    }

    /// <summary>
    /// Returns closest enemy game object of a specific faction.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public GameObject ClosestFactionType(Faction type)
    {
        GameObject closestEnemy = null;
        float currentMinDistance = Mathf.Infinity;

        // Analize all detected enemies
        foreach (Faction current in DetectedEnemies)
        {
            // Check if the current faction object belongs to the desired faction
            if (current.FactionName == type.FactionName)
            {
                // Get distance to the enemy
                float distance = Vector3.Distance(transform.position, current.transform.position);

                // First check the enemy is in the detection range and is the closest distance found until now
                if (distance <= detectionRange && distance < currentMinDistance)
                    closestEnemy = current.gameObject;
            }
        }

        return closestEnemy;
    }

    /// <summary>
    /// Returns random enemy game object within detection range.
    /// </summary>
    /// <returns></returns>
    public GameObject RandomEnemy()
    {
        GameObject randomEnemy = null;

        // Analize all detected enemies
        List<Faction> possibleEnemies = new List<Faction>();
        foreach (Faction current in DetectedEnemies)
        {
            // Get distance to the enemy
            float distance = Vector3.Distance(transform.position, current.transform.position);

            // First check the enemy is in the detection range and is the closest distance found until now
            if (distance <= detectionRange)
                possibleEnemies.Add(current);
        }

        // Select random enemy
        if (possibleEnemies.Count > 0)
            randomEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Count - 1)].gameObject;

        return randomEnemy;
    }

    /// <summary>
    /// Returns a random enemy game object of a specific faction, within detection range.    /// </summary>
    /// <returns></returns>
    public GameObject RandomFactionEnemy(Faction type)
    {
        GameObject randomEnemy = null;

        // Analize all detected enemies
        List<Faction> possibleEnemies = new List<Faction>();
        foreach (Faction current in DetectedEnemies)
        {
            // Check if the current faction object belongs to the desired faction
            if (current.FactionName == type.FactionName)
            {
                // Get distance to the enemy
                float distance = Vector3.Distance(transform.position, current.transform.position);

                // First check the enemy is in the detection range and is the closest distance found until now
                if (distance <= detectionRange)
                    possibleEnemies.Add(current);
            }
        }

        // Select random enemy
        if (possibleEnemies.Count > 0)
            randomEnemy = possibleEnemies[Random.Range(0, possibleEnemies.Count - 1)].gameObject;

        return randomEnemy;
    }

    #endregion

    #region Debug

    /// <summary>
    /// Executed on the draw gizmos event
    /// </summary>
    void OnDrawGizmos()
    {
        // Shows the detection range in editor
        if (DisplayDetectionRange)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, detectionRange);
        }

        // Shows the attack range in editor
        if (DisplayAttackRange)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, attackRange);
        }

        // Shows the avoid range in editor
        if (DisplayAvoidRange)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, avoidRange);
        }
    }

    #endregion

    #region Properties
    public float DetectionRange
    {
        get { return detectionRange; }
        private set { detectionRange = value; }
    }

    public float AttackRange
    {
        get { return attackRange; }
        private set { attackRange = value; }
    }

    public float AvoidRange
    {
        get { return avoidRange; }
        private set { avoidRange = value; }
    }

    public float DetectionRate
    {
        get { return detectionRate; }
        private set { detectionRate = value; }
    }

    #endregion
}
