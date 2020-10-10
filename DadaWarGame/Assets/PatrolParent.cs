using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolParent : MonoBehaviour
{
    [HideInInspector]
    public List<NavMeshAgent> agentsOnThisPatrol;
    // Start is called before the first frame update
    void Start()
    {
        agentsOnThisPatrol = new List<NavMeshAgent>();

        foreach (Transform child in transform)
        {
            NavMeshAgent thisAgent = child.GetComponent<NavMeshAgent>();
            if (thisAgent)
            {
                agentsOnThisPatrol.Add(thisAgent);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
