using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Archer : MonoBehaviour
{
    //Serialized Fields
    [SerializeField]
    float range;

    //Private
    //GameObject[] targetsInitial;
    GameObject[] targets;
    //List<GameObject> targets;
    NavMeshAgent[] targetNavMeshAgents;
    //List<NavMeshAgent> targetNavMeshAgents;

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
    float recalibrateTargetsInterval = 2;
    //public string enemyTag = "AI";

    void Start()
    {
        //targetmanagersection
        //if(tagToShoot == "AI")//if you are shooting AI, whih is player units, ie you are enemy
        //{
        //    targets = TargetsManager.targetsUsedByEnemy;
        //    targetNavMeshAgents = TargetsManager.targetNavMeshAgentsUsedByEnemy;
        //}
        //else if (tagToShoot == "EnemyAI")//if you are shooting enemy ai, ie you are player unit
        //{
        //    targets = TargetsManager.targets;
        //    targetNavMeshAgents = TargetsManager.targetNavMeshAgents;
        //}
        //end targetmanagersection

        //if you want to spawn more enemies later and be able to shoot them you will have to check for targets again when that happens
        targets = GameObject.FindGameObjectsWithTag(tagToShoot);
        //targets = new List<GameObject>();
        //for (int i = 0; i < targetsInitial.Length; i++)
        //{
        //    targets.Add(targetsInitial[i]);
        //}

        audioSources = GetComponents<AudioSource>();
        layerMaskToShootThrough = 1 << 10;
        targetNavMeshAgents = new NavMeshAgent[targets.Length];//new List<NavMeshAgent>();
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
            if (currentTarget /*&& currentTarget.activeSelf*/)
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

    void Update()
    {
        timerForRecalibrateTargets += Time.deltaTime;

        if(timerForRecalibrateTargets >= recalibrateTargetsInterval)
        {
            timerForRecalibrateTargets = 0;
            //need put if this .active self around all stuff in this and fixed update?
            //***main performance hits here are from acquiring active status
            if (!currentTarget /*|| !currentTarget.activeSelf*/)//if current target is null or inactive
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i] /*&& targets[i].activeSelf*/)
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
                            if (Physics.Raycast(this.transform.position, direction, out hit, 100, ~layerMaskToShootThrough))
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
                //might be greater sourceo fperfoamnce hits if archers have already acquired targets

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

