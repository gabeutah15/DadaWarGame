﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class Archer : MonoBehaviour
{
    //Serialized Fields
    [SerializeField]
    float range;

    //Private
    GameObject[] targets;
    NavMeshAgent[] targetNavMeshAgents;
    GameObject currentTarget;
    float timer;
    float targetDistance;
    [SerializeField]
    string tagToShoot;
    [SerializeField]
    public bool holdFire { get; set; }
    int currentTargetIndex;
    [SerializeField]
    LayerMask layerMaskToShootThrough;
    private AudioSource[] audioSources;
    [SerializeField]
    bool isPhysicsProjectile = false;
    [SerializeField]
    GameObject physicsBall;
    float timerForRecalibrateTargets = 0;
    float recalibrateTargetsInterval = .2f;
    EnemyAIBody aiBody;
    //public string enemyTag = "AI";

    void Start()
    {
        aiBody = GetComponent<EnemyAIBody>();
        //if (aiBody)
        //{
        //    aiBody.territory.playerUnitsInYourTerritory.Count > 0;
        //}
        //if you want to spawn more enemies later and be able to shoot them you will have to check for targets again when that happens
        targets = GameObject.FindGameObjectsWithTag(tagToShoot);
        audioSources = GetComponents<AudioSource>();
        //layerMaskToShootThrough = 1 << 10;
        targetNavMeshAgents = new NavMeshAgent[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            //not all enemy agents have navmeshes though
            //but can't mess up ordering
            NavMeshAgent thisAgentsNavMeshAgent = targets[i].gameObject.GetComponent<NavMeshAgent>();
            if (thisAgentsNavMeshAgent)
            {
                targetNavMeshAgents[i] = thisAgentsNavMeshAgent;
            }
            else
            {
                targetNavMeshAgents[i] = null;
            }
        }

        holdFire = false;
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 2)
        {
            timer -= 2;
            if (currentTarget && currentTarget.activeSelf)
            {
                if (!holdFire)
                {
                    if (!isPhysicsProjectile)
                    {
                        ShootProjectile(currentTarget);
                    }
                    else
                    {
                        ShootPhysicsProjectile(currentTarget);
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        timerForRecalibrateTargets += Time.deltaTime;

        //so ai archers don't bother looking for targets if not units in their territory, seems to work,
        //meant as an optimization on large numbers of friendly units and enemy archers (each enemy archer doing this
        //below loop for each friendly unit
        //**would probbaly be better at this point to split out into a friendly and enemy archer class
        //andthe enemy archer class only ever loops through units in its territory
        if (aiBody)
        {
            if(aiBody.territory.playerUnitsInYourTerritory.Count == 0)
            {
                timerForRecalibrateTargets = 0;//if nobody in your territory don't look for targets
                currentTarget = null;
            }
        }

        if(timerForRecalibrateTargets >= recalibrateTargetsInterval)
        {
            timerForRecalibrateTargets = 0;
            //need put if this .active self around all stuff in this and fixed update?
            if (!currentTarget || !currentTarget.activeSelf)//if current target is null or inactive
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] && targets[i].activeSelf/* && targets[i].GetComponent<NavMeshAgent>()*/)
                    {
                        //added this section because some ai enemy targerts have no navmesh, don't move, in which case just shoot at their position
                        Vector3 targetPos = targets[i].transform.position;

                        //don't really need to reference this here just to check range, ok to just chekc current position as optimization
                        //NavMeshAgent enemyAgent = targetNavMeshAgents[i];//using this is optimazation but onluy incidentally aligns with right target in targets[i]// targets[i].GetComponent<NavMeshAgent>();
                        //if (enemyAgent)
                        //{
                            //targetPos = enemyAgent.nextPosition;
                        //}

                        float distance = Vector3.Distance(this.transform.position, targetPos);

                        //this could be changed to make them pick a kind of random target within range rather than just the first one, because right now
                        //all of the archers end up shooting at the same target and then switching to a new target
                        if (distance < range)
                        {
                            //no sight section
                            //tested just doing this to test performance but lag not caused by raycast
                            //int rand = Random.Range(0, 5);
                            //if (rand == 0)
                            //{
                            //    currentTarget = targets[i];
                            //    currentTargetIndex = i;
                            //    targetDistance = distance;
                            //}
                            //end no sight

                            RaycastHit hit;
                            Vector3 thisPosition = this.transform.position;
                            Vector3 direction = targetPos - thisPosition;
                            if (Physics.Raycast(thisPosition, direction, out hit, 100, ~layerMaskToShootThrough))
                            {
                                GameObject targetObject = hit.collider.gameObject;
                                if(targetObject == targets[i])
                                {
                                    //Debug.Log("Can See Enemy");
                                    //this randomization actually works decently to make targets more random and not have all the units shoot at the same target
                                    int rand = Random.Range(0, 5);
                                    if(rand == 0)
                                    {
                                        currentTarget = targets[i];
                                        currentTargetIndex = i;
                                        targetDistance = distance;
                                    }
                                }
                                else
                                {
                                    //Debug.Log("Can NOT See Enemy");
                                }
                            }

                        }
                    }
                }
            }
            else
            {
                //similar added for non mesh agent enemies
                Vector3 targetPos = currentTarget.transform.position;

                NavMeshAgent enemyAgent = targetNavMeshAgents[currentTargetIndex]; //currentTarget.GetComponent<NavMeshAgent>();//find this on index as well?
                if (enemyAgent)
                {
                    targetPos = enemyAgent.nextPosition;
                }

                float distance = Vector3.Distance(this.transform.position, targetPos);
                if (distance > range)
                {
                    currentTarget = null;
                }

                RaycastHit hit;
                if (currentTarget)
                {
                    Vector3 direction = currentTarget.transform.position - this.transform.position;
                    if (Physics.Raycast(this.transform.position, direction, out hit, 100, ~layerMaskToShootThrough))
                    {
                        GameObject targetObject = hit.collider.gameObject;
                        if (targetObject == currentTarget)
                        {
                                //do nothing because you can see them, if can't see then set null
                                //should set these on layer mask that only sees enemies and walls, so they don't block one another's line of sight
                        }
                        else
                        {
                            currentTarget = null;
                        }
                    }
                }
            }

        }
    }

    public void ShootProjectile(GameObject targetUnit)
    {
        //if lead ball then need to set initialize position as higher up and further forward
        Transform p = AmmoManager.SpawnAmmo(this.transform.position, Quaternion.identity, this.name);
        if (this.name.Contains("Catapult"))
        {
            if (audioSources != null && audioSources.Length == 3)
            {
                audioSources[2].Play();
            }
        }

        //similar added for non mesh agent enemies
        Vector3 targetPos = targetUnit.transform.position;

        NavMeshAgent enemyAgent = targetNavMeshAgents[currentTargetIndex];//target unit is current target //targetUnit.GetComponent<NavMeshAgent>();
        if (enemyAgent)
        {
            targetPos = enemyAgent.nextPosition;
            if (enemyAgent.velocity.sqrMagnitude < .1f)
            {
                /*targetPosition*/ targetPos = targetUnit.transform.position;
            }
        }

        //Vector3 targetPosition = targetUnit.GetComponent<NavMeshAgent>().nextPosition;


        p.GetComponent<Projectile>().Initialize(/*targetPosition*/targetPos, enemyAgent, targetDistance);
    }

    public void ShootPhysicsProjectile(GameObject target)
    {
        if (physicsBall)
        {
            GameObject thisPhysicsBall = Instantiate(physicsBall, transform.position + new Vector3(0,2,0), Quaternion.identity);
            AddForceTest force = thisPhysicsBall.GetComponent<AddForceTest>();
            force.target = target;
            force.Throw();
        }
    }
}

