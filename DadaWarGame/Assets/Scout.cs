using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Scout : MonoBehaviour
{
    [HideInInspector]
    public bool IsReturningToGeneral;
    NavMeshAgent agent;
    ScoutManager scoutManager;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        scoutManager = FindObjectOfType<ScoutManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsReturningToGeneral)
        {
            if (!agent.pathPending)
            {
                if (agent.remainingDistance < 5)
                {
                    scoutManager.ConsumeScout(this.gameObject);
                }
            }
        }
    }
}
