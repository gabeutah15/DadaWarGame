using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public enum unitTypeToSpawn
    {
        Archer = 1,
        Spearman
    }

    GameObject spawnPrefab;
    [SerializeField]
    GameObject archerPrefab;
    [SerializeField]
    GameObject spearmanPrefab;

    float formationXOffset = 0;
    float formationZOffset = 0;

    //private int numFormations = 3;
    private int numRows = 4;
    private int numColumns = 4;
    private int totalNumFormationsSpawned = 0;

    public int numArchers { get; set; }
    public int numSpearmen { get; set; }

    bool hasStartedSecondRow = false;

    // put in awake so other stuff can find in start
    void Awake()
    {
        numArchers = 6;
        numSpearmen = 6;
        SpawnUnits();
    }

    private void SpawnUnits()
    {
        SpawnFormation(numSpearmen, spearmanPrefab);
        SpawnFormation(numArchers, archerPrefab);

    }

    private void SpawnFormation(int numFormationsThisUnitType, GameObject prefab)
    {
        for (int x = 0; x < numFormationsThisUnitType; x++)
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    GameObject thisUnit = Instantiate(prefab, new Vector3(-20, 0.3f, -100) + new Vector3(i * 3 + formationXOffset, 0, j * 3 - formationZOffset), Quaternion.identity) as GameObject;
                    if (thisUnit.GetComponent<AIControl>())
                    {
                        AIControl control = thisUnit.GetComponent<AIControl>();
                        control.selectedUnitNum = totalNumFormationsSpawned + 1;
                    }
                }
            }
            totalNumFormationsSpawned++;
            formationXOffset += 13;
            if((totalNumFormationsSpawned) > 5 && !hasStartedSecondRow)
            {
                formationZOffset = 15;
                formationXOffset = 0;
                hasStartedSecondRow = true;
            }
        }
    }
}
