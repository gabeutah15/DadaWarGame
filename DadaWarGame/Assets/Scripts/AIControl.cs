using System;
using System.Collections;
using System.Collections.Generic;
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
    GameObject currentTarget;
    [SerializeField]
    bool isMeleeUnit;
    [HideInInspector]
    public bool isAwaitingOrders;
    [HideInInspector]
    public Vector3 futureDestination;
    
    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        capsuleCollider = this.GetComponent<Collider>();
        foreach (Transform child in transform)
        {
            if (child.tag == "Highlight")
                highlight = child.gameObject;
        }
        enemyAgents = GameObject.FindGameObjectsWithTag("EnemyAI");
        enemyNavMeshAgents = new NavMeshAgent[enemyAgents.Length];
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
        }

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

        if((agent.remainingDistance < 10) && isMeleeUnit)
        {
            PursueNearest();
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
        //}
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
                    if ((distance < minDistance) && (distance < 10) /*&& ((distance < 5) || (enemyIsArcher && (distance < 10)))*/)//hardcoded less than ten so only does this at all if close to enemies even if has no 'remaining distance'
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
            agent.SetDestination(currentTarget.transform.position);
        }
        if (agent.remainingDistance > (agent.stoppingDistance + 1))
        {
            this.transform.LookAt(agent.steeringTarget + new Vector3(0, .5f, 0));
        }
    }
}
