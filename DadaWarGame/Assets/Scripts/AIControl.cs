﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    NavMeshAgent[] enemyNavMeshAgents;
    bool[] enemyIsGateHouse;
    GameObject currentTarget;
    [SerializeField]
    bool isMeleeUnit;
    private AudioSource[] sounds;
    [HideInInspector]
    public bool isAwaitingOrders;
    [HideInInspector]
    public Vector3 futureDestination;
    [SerializeField]
    int independentPursueDistance = 5;
    Animator animator;
    public float lineOfSight = 50;
    [SerializeField]
    private bool IsScout;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        agent = this.GetComponent<NavMeshAgent>();
        capsuleCollider = this.GetComponent<Collider>();
        foreach (Transform child in transform)
        {
            if (child.tag == "Highlight")
                highlight = child.gameObject;
        }
        enemyAgents = GameObject.FindGameObjectsWithTag("EnemyAI");
        enemyNavMeshAgents = new NavMeshAgent[enemyAgents.Length];
        enemyIsGateHouse = new bool[enemyAgents.Length];
        for (int i = 0; i < enemyAgents.Length; i++)
        {
            //not all enemy agents have navmeshes though
            //but can't mess up ordering
            NavMeshAgent thisAgentsNavMeshAgent = enemyAgents[i].gameObject.GetComponent<NavMeshAgent>();
            if (thisAgentsNavMeshAgent)
            {
                enemyNavMeshAgents[i] = thisAgentsNavMeshAgent;

            }
            else
            {
                enemyNavMeshAgents[i] = null;
            }

            GateHouse thisGateHouse = enemyAgents[i].gameObject.GetComponent<GateHouse>();
            Gate thisGate = enemyAgents[i].gameObject.GetComponent<Gate>();
            GateDestroyed thisGateDestroyed = enemyAgents[i].gameObject.GetComponent<GateDestroyed>();

            if (thisGateHouse || thisGate || thisGateDestroyed)
            {
                enemyIsGateHouse[i] = true;
            }
            else
            {
                enemyIsGateHouse[i] = false;
            }
        }

        sounds = GetComponents<AudioSource>();   //assumes 'die sound' is first and 'charge ahead' sound is second
        //dieSound = GetComponent<AudioSource>();
        currentTarget = null;
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectileParent = collision.gameObject.GetComponentInParent<Projectile>();
            if (projectileParent)
            {
                if (projectileParent.isDeadly)
                {
                    health--;
                }
            }
        }
        if (collision.gameObject.CompareTag("EnemySword"))
        {
            health--;
        }
        //if(this.name == "Catapult (1)")
        //UnityEngine.Debug.Log("health .." + health+ "health == 0: " + (health == 0)+ " dieSound.enabled: "+ dieSound.enabled);
        if (health == 0 && sounds != null && sounds.Length > 0 && sounds[0].enabled)
        {
            sounds[0].Play();
            //StartCoroutine(playDieSound());
        }
        if (health <= 0)
        {
            this.gameObject.SetActive(false);//destroy or just setactive false?
            //Destroy(this.gameObject);
        }
    }


    /*private IEnumerator playDieSound()
    {
        *//*dieSound.Play();*//*
        while (dieSound.isPlaying)
        {
            yield return null;
        }
    }*/
    private float recalibrateTimer = 0;
    private float recalibrateInterval = 1f;

    private void Update()
    {
        if (animator)
        {
            if (agent.velocity.sqrMagnitude > 0.1f)//a
            {
                animator.SetBool("IsMoving", true);
            }
            else
            {
                animator.SetBool("IsMoving", false);
            }
        }


        //if (UnitSelectionManager.selectedUnits)//dunno if this is the nullcheck here?
        //{
        if (UnitSelectionManager.selectedUnits.Contains((SelectedUnit)selectedUnitNum))//null ref here?
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
        if (health <= 0 && sounds != null && sounds.Length > 0 && !sounds[0].isPlaying && sounds[0].enabled)
        {
            UnityEngine.Debug.Log("Playing stopped ..");
            sounds[0].enabled = false;
            this.gameObject.SetActive(false);
        }

        recalibrateTimer += Time.deltaTime;
        if (recalibrateTimer > recalibrateInterval)
        {
            recalibrateTimer = 0;
            if (/*!IsScout && */agent.remainingDistance > 5)//probably remove isscout ref later? well won't matter with 2d art and no attacking and no formation
            {
                this.transform.LookAt(agent.steeringTarget + new Vector3(0, .5f, 0));
                // Debug.DrawRay(this.transform.position, agent.steeringTarget + new Vector3(0, .5f, 0));//trying to debug why this lookat makes them flip to the ground when near the target destination
            }

            if (isMeleeUnit && (agent.remainingDistance < independentPursueDistance))
            {
                PursueNearest();
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
                if (enemyAgents[i].activeSelf && enemyIsGateHouse[i] == false)
                {
                    //bool enemyIsArcher = true;
                    Vector3 enemyPosition = enemyAgents[i].transform.position;//if archer ie no nav mesh agent

                    //get component in a loop like this in update is extrememly non performant
                    NavMeshAgent enemyAgent = enemyNavMeshAgents[i];//i's should line up here// enemyAgents[i].GetComponent<NavMeshAgent>();

                    if (enemyAgent)
                    {
                        enemyPosition = enemyAgent.nextPosition;//if swordsmen ie has agent
                        //enemyIsArcher = false;//this is only incidentally correct
                    }

                    float distance = Vector3.Distance(this.transform.position, enemyPosition);
                    if ((distance < minDistance) && (distance < independentPursueDistance) /*&& ((distance < 5) || (enemyIsArcher && (distance < 10)))*/)//hardcoded less than ten so only does this at all if close to enemies even if has no 'remaining distance'
                    {
                        //could instead do some bool for 'isUnderFire' in which case a unit will, if given no other order, always pursue and attack archers that can shoot at it
                        //kind of makes sense but might also be annoying
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
            //agent.SetDestination(currentTarget.transform.position);
            SetDestinationIfAttainable(agent, currentTarget.transform.position);
        }
        else
        {
            agent.SetDestination(agent.transform.position);//if your target is null and you were pursuing nearest then just stop
        }

        if (agent.remainingDistance > (agent.stoppingDistance + 1))
        {
            this.transform.LookAt(agent.steeringTarget + new Vector3(0, .5f, 0));
        }
    }

    private void SetDestinationIfAttainable(NavMeshAgent agent, Vector3 destination)
    {
        NavMeshPath path = new NavMeshPath();
        agent.CalculatePath(destination, path);
        if (path.status == NavMeshPathStatus.PathPartial)
        {
            //do something if agent cannot reach destination
        }
        else
        {
            agent.SetDestination(destination);
        }
    }
}
