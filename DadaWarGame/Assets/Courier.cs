using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Courier : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    public GameObject target;
    public NavMeshAgent targetsNavMeshAgent;
    public int targetUnitNum;
    //public bool available;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
