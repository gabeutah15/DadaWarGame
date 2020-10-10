using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBody : MonoBehaviour
{
    //Collider collider;
    [SerializeField]
    public float health = 1;
    Animator animator;
    NavMeshAgent agent;
    GameObject[] playerUnits;
    NavMeshAgent[] playerNavMeshAgents;
    GameObject currentTarget;
    [SerializeField]
    public GameObject deathModelPrefab;
    [SerializeField]
    public Territory territory;
    Vector3 startingPosition;
    Quaternion startingRotation;
    bool hasSetToReturnToStart = false;

    // Start is called before the first frame update
    void Start()
    {
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
                    SetDestinationIfAttainable(agent, startingPosition);
                    hasSetToReturnToStart = true;
                }

                if (agent.remainingDistance < .2)
                {
                    if (hasSetToReturnToStart)
                    {
                        this.transform.rotation = startingRotation;
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
            health--;

        }

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

            AddForceTest physicsBall = collision.gameObject.GetComponent<AddForceTest>();
            if (physicsBall)
            {
                if (physicsBall.isDeadly)
                {
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
