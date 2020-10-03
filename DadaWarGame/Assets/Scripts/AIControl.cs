﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour
{
    public NavMeshAgent agent;
    Collider capsuleCollider;
    [SerializeField]
    float health;
    [SerializeField]
    public int selectedUnitNum;
    GameObject highlight;
    GameObject[] enemyAgents;
    GameObject currentTarget;
    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        capsuleCollider = this.GetComponent<Collider>();
        foreach (Transform child in transform)
        {
            if (child.tag == "Highlight") ;
                highlight = child.gameObject;
        }
        enemyAgents = GameObject.FindGameObjectsWithTag("EnemyAI");
        currentTarget = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Projectile"))
        {
            //Debug.Log("hit with arrow");
            health--;
        }

        if (collision.gameObject.CompareTag("EnemySword"))
        {
            health--;
        }

        if (health <= 0)
        {
            this.gameObject.SetActive(false);//destroy or just setactive false?
            //Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if(agent.remainingDistance > 5)
        {
            this.transform.LookAt(agent.steeringTarget + new Vector3(0, .5f, 0));
            Debug.DrawRay(this.transform.position, agent.steeringTarget + new Vector3(0, .5f, 0));//trying to debug why this lookat makes them flip to the ground when near the target destination
        }

        if(agent.remainingDistance < 5)
        {
            PursueNearest();
        }

        if (selectedUnitNum == (int)UnitSelectionManager.selectedUnit)
        {
            if (!highlight.activeSelf)
            {
                highlight.SetActive(true);
            }
        }
        else
        {
            if (highlight.activeSelf)
            {
                highlight.SetActive(false);
            }
        }
    }

    private void PursueNearest()
    {
        if (!currentTarget || !currentTarget.activeSelf)
        {
            float minDistance = int.MaxValue;
            GameObject potentialTarget = null;

            for (int i = 0; i < enemyAgents.Length; i++)
            {
                if (enemyAgents[i].activeSelf)
                {
                    Vector3 enemyPosition = enemyAgents[i].transform.position;//if archer ie no nav mesh agent

                    if (enemyAgents[i].GetComponent<NavMeshAgent>())
                    {
                        enemyPosition = enemyAgents[i].GetComponent<NavMeshAgent>().nextPosition;//if swordsmen ie has agent
                    }

                    float distance = Vector3.Distance(this.transform.position, enemyPosition);
                    if ((distance < minDistance) && (distance < 5))//hardcoded less than five so only does this at all if close to enemies even if has no 'remaining distance'
                    {
                        minDistance = distance;
                        potentialTarget = enemyAgents[i];//find closest enemyAgents that is active and not destroyed
                    }
                }
            }

            if (potentialTarget)
            {
                currentTarget = potentialTarget;
                //agent.SetDestination(currentTarget.transform.position);
            }

            if (currentTarget)
            {
                if (!currentTarget.activeSelf)
                {
                    //if you have a current target but it is not active set it to null to ensure you find a new one
                    currentTarget = null;
                }
            }
        }

        if (currentTarget)
        {
            agent.SetDestination(currentTarget.transform.position);
        }
        if (agent.remainingDistance > (agent.stoppingDistance + 1))
        {
            this.transform.LookAt(agent.steeringTarget + new Vector3(0, .5f, 0));
        }
    }
}
