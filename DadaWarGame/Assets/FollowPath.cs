using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FollowPath : MonoBehaviour
{
    [SerializeField]
    Transform[] wayPoints;
    NavMeshAgent agent;

    int destinationIndex;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    public void GoToDestination(int index)
    {
        agent.SetDestination(wayPoints[index].position);
    }

    // Update is called once per frame
    void Update()
    {
        if(agent.remainingDistance < 0.1)
        {
            destinationIndex++;
            destinationIndex = destinationIndex % wayPoints.Length;
            GoToDestination(destinationIndex);
        }
    }
}
