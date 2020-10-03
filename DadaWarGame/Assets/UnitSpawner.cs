using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject spawnPrefab;
    float formationOffset = 0;
    private int numFormations = 3;
    private int numRows = 3;
    private int numColumns = 3;

    // Update is called once per frame
    void Start()
    {
        for (int x = 0; x < numFormations; x++)
        {
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    GameObject thisUnit = Instantiate(spawnPrefab, Vector3.zero + new Vector3(i * 3 + formationOffset, 0, j * 3), Quaternion.identity) as GameObject;
                    if (thisUnit.GetComponent<AIControl>())
                    {
                        AIControl control = thisUnit.GetComponent<AIControl>();
                        control.selectedUnitNum = x + 1;
                    }
                }
            }
            formationOffset += 10;
        }
    }
}
