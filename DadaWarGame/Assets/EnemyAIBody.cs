using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBody : MonoBehaviour
{
    //Collider collider;
    [SerializeField]
    float health = 1;
    Animator animator;
    NavMeshAgent agent;
    GameObject[] playerUnits;
    NavMeshAgent[] playerNavMeshAgents;
    GameObject currentTarget;
    [SerializeField]
    GameObject deathModelPrefab;
    [SerializeField]
    public Territory territory;
    Vector3 startingPosition;
    Quaternion startingRotation;
    bool hasSetToReturnToStart = false;
    [SerializeField]
    bool IsPatroller;
    [SerializeField]
    GameObject bloodParticle;

    //added for patrolling:
    Vector3[] wayPoints;
    [SerializeField]
    int addedXDistanceForPatrolPoint;
    [SerializeField]
    int addedYDistanceForPatrolPoint;
    [SerializeField]
    int addedZDistanceForPatrolPoint;
    PatrolParent patrolParent;
    int destinationIndex = 1;
    //[SerializeField]
    //EnemyAIBody[] EnemiesInTeam;//don't need to track this, just give them all the patrol points based off of their starting point
    //end added for patrolling

    // Start is called before the first frame update
    void Start()
    {
        patrolParent = GetComponentInParent<PatrolParent>();
        //collider = this.GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();//***TODO: put all getcomponent calls into awake not start methods
        agent = GetComponent<NavMeshAgent>();
        playerUnits = GameObject.FindGameObjectsWithTag("AI");
        playerNavMeshAgents = new NavMeshAgent[playerUnits.Length];
        for (int i = 0; i < playerUnits.Length; i++)
        {
            //not all enemy agents have navmeshes though
            //but can't mess up ordering
            NavMeshAgent thisAgentsNavMeshAgent = playerUnits[i].gameObject.GetComponent<NavMeshAgent>();
            if (thisAgentsNavMeshAgent)
            {
                playerNavMeshAgents[i] = thisAgentsNavMeshAgent;

            }
            else
            {
                playerNavMeshAgents[i] = null;
            }
        }

        currentTarget = null;
        territory.thisTerritorysDefenders.Add(this.gameObject);
        startingPosition = this.transform.position;
        startingRotation = this.transform.rotation;

        wayPoints = new Vector3[2];
        wayPoints[0] = startingPosition;

        NavMeshHit myNavHit;
        Vector3 firstWaypoint = startingPosition + new Vector3(addedXDistanceForPatrolPoint, addedYDistanceForPatrolPoint, addedZDistanceForPatrolPoint);
        if (NavMesh.SamplePosition(firstWaypoint, out myNavHit, 100, -1))
        {
            firstWaypoint = myNavHit.position;
        }

        wayPoints[1] = firstWaypoint;// startingPosition + new Vector3(addedXDistanceForPatrolPoint, addedYDistanceForPatrolPoint, addedZDistanceForPatrolPoint);
    }

    public void GoToDestination(int index)
    {
        SetDestinationIfAttainable(agent, wayPoints[index]);
    }

    // Update is called once per frame
    void CheckIfArrivingAtWayPoint()
    {
        bool everyOneHassArrived = true;
        foreach (var otherAgent in patrolParent.agentsOnThisPatrol)
        {
            if (otherAgent.gameObject.activeSelf)
            {
                if(otherAgent.remainingDistance > 1f)//what about dead agents though? will they be forever at a certain remaining distanc? 
                {
                    everyOneHassArrived = false;
                }
            }
            else
            {
                //remove from patrol parent? don't really need to remove though...if already checking if active before doing anything, and can't remove inside loop
            }
        }
        if (everyOneHassArrived && (agent.remainingDistance < 0.1))
        {
            destinationIndex++;
            destinationIndex = destinationIndex % wayPoints.Length;
            GoToDestination(destinationIndex);
        }
    }

    float recalibrateTimer = 0;
    float recalibrateInterval = 2;

    private void Update()
    {
        recalibrateTimer += Time.deltaTime;
        if (recalibrateTimer > recalibrateInterval)
        {
            recalibrateTimer = 0;

            if (agent)//basically if it's an enemy that moves, right now not an enemy archer
            {

                if (!currentTarget || !currentTarget.activeSelf)
                {
                    float minDistance = int.MaxValue;
                    GameObject potentialTarget = null;

                    for (int i = 0; i < playerUnits.Length; i++)
                    {
                        if (playerUnits[i].activeSelf && playerNavMeshAgents[i] && territory.playerUnitsInYourTerritory.Contains(playerUnits[i]))
                        {
                            float distance = Vector3.Distance(this.transform.position, playerNavMeshAgents[i].nextPosition);
                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                potentialTarget = playerUnits[i];//find closest playeragent that is active and not destroyed
                            }
                        }
                    }

                    if (potentialTarget)
                    {
                        currentTarget = potentialTarget;
                        hasSetToReturnToStart = false;
                        //agent.SetDestination(currentTarget.transform.position);
                    }

                    if (currentTarget)
                    {
                        if (!currentTarget.activeSelf)
                        {
                            hasSetToReturnToStart = false;
                            currentTarget = null;
                        }
                    }
                }

                //should do something to switch to closer targets in territory maybe? or probably just make territories smaller
                if (currentTarget && territory.playerUnitsInYourTerritory.Contains(currentTarget))
                {
                    //agent.SetDestination(currentTarget.transform.position);
                    SetDestinationIfAttainable(agent, currentTarget.transform.position);
                }

                if (currentTarget && !territory.playerUnitsInYourTerritory.Contains(currentTarget))
                {
                    //agent.SetDestination(startingPosition);
                    SetDestinationIfAttainable(agent, startingPosition);
                    currentTarget = null;
                }

                if (agent.remainingDistance > 3)
                {
                    this.transform.LookAt(agent.steeringTarget + new Vector3(0, .5f, 0));
                }

                //don't have any targets and you are close to last target ie last enemy you killed then go back
                if (currentTarget == null && (agent.remainingDistance < 5) && !hasSetToReturnToStart)
                {
                    //agent.SetDestination(startingPosition);
                    if (!IsPatroller)
                    {
                        SetDestinationIfAttainable(agent, startingPosition);
                        hasSetToReturnToStart = true;//dunno if need to set this back to false later?
                    }
                    else
                    {
                        GoToDestination(destinationIndex);
                    }

                }
                
                if((currentTarget == null) && !hasSetToReturnToStart && IsPatroller)
                {
                    CheckIfArrivingAtWayPoint();
                }

                if (agent.remainingDistance < .2)
                {
                    if (hasSetToReturnToStart)
                    {
                        this.transform.rotation = startingRotation;
                        hasSetToReturnToStart = false;
                    }
                }

                if (/*(agent.remainingDistance > 0) &&*/ (agent.remainingDistance < 5) && (currentTarget != null))//a
                {
                    animator.SetBool("IsInCombat", true);
                }
                else
                {
                    animator.SetBool("IsInCombat", false);

                }
            }
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

    private void OnCollisionEnter(Collision collision)
    {
        //ContactPoint[] contacts = collision.contacts;

        //for (int i = 0; i < contacts.Length; i++)
        //{

        //}

        //would like to do this so only if collision from player spear on ai body, not on shield
        var parent = collision.gameObject.GetComponentInParent<AIControl>();
        if (parent)
        {
            //Debug.Log("collided with child of ai control");
            Instantiate(bloodParticle, transform.position + new Vector3(0,0,-0.5f), Quaternion.identity);
            health--;

        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectileParent = collision.gameObject.GetComponentInParent<Projectile>();
            if (projectileParent)
            {
                if (projectileParent.isDeadly)
                {
                    Instantiate(bloodParticle, transform.position + new Vector3(0, 0, -0.5f), Quaternion.identity);
                    health--;
                }
            }

            AddForceTest physicsBall = collision.gameObject.GetComponent<AddForceTest>();
            if (physicsBall)
            {
                if (physicsBall.isDeadly)
                {
                    Instantiate(bloodParticle, transform.position + new Vector3(0, 0, -0.5f), Quaternion.identity);
                    health -= 2;
                }
            }
        }

        //this doesn't work for finding just collision with playerweapon (spear) and nor does below with compare tag
        //need rigidbody for collision on navagent, can only have one rigidbody on object and children combined
        //var spear = collision.gameObject.GetComponent<PlayerWeapon>();
        //if (spear)
        //{
        //    Debug.Log("collision with spear");
        //}


        //Collider myCollider = collision.contacts[0].thisCollider;//this should get actually contacted collider?

        //if (myCollider.gameObject.CompareTag("PlayerWeapon"))
        //{
        //    Debug.Log("hit by weapon of player unit");
        //    health--;
        //}

        if (health <= 0)
        {
            DeathCounterAndRandomNames.totalDeadEnemies++;

            this.gameObject.SetActive(false);
            if (deathModelPrefab)
            {
                GameObject thisUnitDead = Instantiate(deathModelPrefab, this.transform.position, this.transform.rotation) as GameObject;
                thisUnitDead.GetComponent<Rigidbody>().AddForce(new Vector3(1f, 0, 0.5f));//just enough to knock it over
                deathModelPrefab = null;
            }

            //Destroy(this.gameObject);
        }
    }

}
