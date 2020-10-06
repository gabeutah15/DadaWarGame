using System.Collections;
using System.Collections.Generic;
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

    //public string enemyTag = "AI";

    void Start()
    {
        //if you want to spawn more enemies later and be able to shoot them you will have to check for targets again when that happens
        targets = GameObject.FindGameObjectsWithTag(tagToShoot);

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
                    ShootProjectile(currentTarget);
                }
            }
        }
    }

    void Update()
    {
        //need put if this .active self around all stuff in this and fixed update?
        if (!currentTarget || !currentTarget.activeSelf)//if current target is null or inactive
        {
            for (int i = 0; i < targets.Length; i++)
            {
                if (targets[i] && targets[i].activeSelf/* && targets[i].GetComponent<NavMeshAgent>()*/)
                {
                    //added this section because some ai enemy targerts have no navmesh, don't move, in which case just shoot at their position
                    Vector3 targetPos = targets[i].transform.position;

                    NavMeshAgent enemyAgent = targetNavMeshAgents[i];//using this is optimazation but onluy incidentally aligns with right target in targets[i]// targets[i].GetComponent<NavMeshAgent>();

                    if (enemyAgent)
                    {
                        targetPos = enemyAgent.nextPosition;
                    }
                    float distance = Vector3.Distance(this.transform.position, targetPos);

                    //this could be changed to make them pick a kind of random target within range rather than just the first one, because right now
                    //all of the archers end up shooting at the same target and then switching to a new target
                    if (distance < range)
                    {

                        RaycastHit hit;
                        Vector3 direction = targets[i].transform.position - this.transform.position;
                        if (Physics.Raycast(this.transform.position, direction, out hit, 100))
                        {
                            GameObject targetObject = hit.collider.gameObject;
                            if(targetObject == targets[i])
                            {
                                //Debug.Log("Can See Enemy");
                                //this randomization actually works decently to make targets more random and not have all the units shoot at the same target
                                int rand = Random.Range(0, 4);
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
                if (Physics.Raycast(this.transform.position, direction, out hit, 100))
                {
                    GameObject targetObject = hit.collider.gameObject;
                    if (targetObject == currentTarget)
                    {
                            //nothing you can see them, if can't see then set null
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

    public void ShootProjectile(GameObject targetUnit)
    {
        //if lead ball then need to set initialize position as higher up and further forward
        Transform p = AmmoManager.SpawnAmmo(this.transform.position, Quaternion.identity, this.name);

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
}

