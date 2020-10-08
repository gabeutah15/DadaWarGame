using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TargetsManager : MonoBehaviour
{

    GameObject[] targetsInitial;
    public static List<GameObject> targets;
    public static List<NavMeshAgent> targetNavMeshAgents;

    GameObject[] targetsInitialUsedByEnemy;
    public static List<GameObject> targetsUsedByEnemy;
    public static List<NavMeshAgent> targetNavMeshAgentsUsedByEnemy;


    void Start()
    {
        targetsInitial = GameObject.FindGameObjectsWithTag("EnemyAI");
        targets = new List<GameObject>();
        for (int i = 0; i < targetsInitial.Length; i++)
        {
            targets.Add(targetsInitial[i]);
        }

        targetNavMeshAgents = new List<NavMeshAgent>();
        for (int i = 0; i < targets.Count; i++)
        {
            var test = targets[i].gameObject;
            NavMeshAgent thisAgentsNavMeshAgent = targets[i].gameObject.GetComponent<NavMeshAgent>();
            if (thisAgentsNavMeshAgent)
            {
                targetNavMeshAgents.Add(thisAgentsNavMeshAgent);
            }
            else
            {
                targetNavMeshAgents.Add(null);
            }
        }


        targetsInitialUsedByEnemy = GameObject.FindGameObjectsWithTag("EnemyAI");
        targetsUsedByEnemy = new List<GameObject>();
        for (int i = 0; i < targetsInitialUsedByEnemy.Length; i++)
        {
            targetsUsedByEnemy.Add(targetsInitialUsedByEnemy[i]);
        }

        targetNavMeshAgentsUsedByEnemy = new List<NavMeshAgent>();
        for (int i = 0; i < targetsUsedByEnemy.Count; i++)
        {
            NavMeshAgent thisAgentsNavMeshAgent = targetsUsedByEnemy[i].gameObject.GetComponent<NavMeshAgent>();
            if (thisAgentsNavMeshAgent)
            {
                targetNavMeshAgentsUsedByEnemy.Add(thisAgentsNavMeshAgent);
            }
            else
            {
                targetNavMeshAgentsUsedByEnemy.Add(null);
            }
        }
    }

    public static void RemoveFromTargetsList(GameObject unitToRemoveFromLists)
    {
        int index = -1;
        if (targets.Contains(unitToRemoveFromLists))
        {
            index = targets.IndexOf(unitToRemoveFromLists);
            targets.Remove(unitToRemoveFromLists);
            targetNavMeshAgents.RemoveAt(index);
        }
    }

    public static void RemoveFromUsedByEnemyTargetsList(GameObject unitToRemoveFromLists)
    {
        int index = -1;
        if (targetsUsedByEnemy.Contains(unitToRemoveFromLists))
        {
            index = targetsUsedByEnemy.IndexOf(unitToRemoveFromLists);
            if (index != -1)//is this necessary?
            {
                targetsUsedByEnemy.Remove(unitToRemoveFromLists);
                targetNavMeshAgentsUsedByEnemy.RemoveAt(index);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
