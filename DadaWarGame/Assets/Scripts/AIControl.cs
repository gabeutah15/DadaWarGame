using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
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

    private int leftAnimHash;
    private int rightAnimHash;
    private int forwardAnimHash;
    private int backwardAnimHash;
    private int isMovingHash;
    private int isDeadHash;

    //[HideInInspector]
    //public bool canBeControlled;


    bool HasGottenNumEnemies = false;

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

        leftAnimHash = Animator.StringToHash("Left2");
        rightAnimHash = Animator.StringToHash("Right2");
        forwardAnimHash = Animator.StringToHash("Forward2");
        backwardAnimHash = Animator.StringToHash("Backward2");
        isMovingHash = Animator.StringToHash("IsMoving");
        isDeadHash = Animator.StringToHash("Death");

        if (animator)
        {
            animator.speed = 1f;
        }
    }

    float deathAnimationTimeElapsed = 0;

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
            isDead = true;
            animator.SetBool(isDeadHash, true);
            agent.speed = 0;
            highlight.SetActive(false);//this is not working
                                       //also the deathanimation time elapsed below is not always working, sometimes takes way more or less time


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

    private float recalibrateTimerAnim = 0;
    private float recalibrateIntervalAnim = .1f;

    //***check if this should be in late update or not?
    private void LateUpdate()
    {
        //recalibrateTimerAnim += Time.deltaTime;



        if (animator)
        {
            recalibrateTimerAnim = 0;

            //probably what's goin on is this script on lots of objects, some with ismoving, some with frontleftbackright, and some with no animator

            if (agent.velocity.sqrMagnitude > 0.1f)//a
            {
                animator.SetBool(isMovingHash, true);
            }
            else
            {
                animator.SetBool(isMovingHash, false);
            }



            Vector3 rotation = (this.transform.rotation * Vector3.forward).normalized;
            float x = rotation.x;
            float z = rotation.z;
            //Debug.Log(rotation);
            if (Mathf.Abs(x) > Mathf.Abs(z))
            {
                //more left or right than forward or back
                if (x < 0)
                {
                    //RIGHT
                    animator.SetBool(leftAnimHash, false);
                    animator.SetBool(rightAnimHash, true);
                    animator.SetBool(forwardAnimHash, false);
                    animator.SetBool(backwardAnimHash, false);
                }
                else
                {
                    //LEFT
                    animator.SetBool(leftAnimHash, true);
                    animator.SetBool(rightAnimHash, false);
                    animator.SetBool(forwardAnimHash, false);
                    animator.SetBool(backwardAnimHash, false);
                }
            }
            else
            {
                //more forward or back
                if (z < 0)
                {
                    //Backward
                    animator.SetBool(leftAnimHash, false);
                    animator.SetBool(rightAnimHash, false);
                    animator.SetBool(forwardAnimHash, false);
                    animator.SetBool(backwardAnimHash, true);
                }
                else
                {
                    //Forward
                    animator.SetBool(leftAnimHash, false);
                    animator.SetBool(rightAnimHash, false);
                    animator.SetBool(forwardAnimHash, true);
                    animator.SetBool(backwardAnimHash, false);
                }
            }

        }
    }

    private bool isDead;

    private void Update()
    {
        //instead of can be controlled just place each unit off the map so you cannot click it until it 'spawns'
        //if (canBeControlled)
        //{



            if (!isDead)
            {


                if (!HasGottenNumEnemies)
                {
                    HasGottenNumEnemies = true;
                    DeathCounterAndRandomNames.totalEnemies = enemyAgents.Length;
                }
                //if (UnitSelectionManager.selectedUnits)//dunno if this is the nullcheck here?
                //{
                if (UnitSelectionManager.selectedUnits.Count > 0)
                {
                    if (selectedUnitNum >= 0)
                    {
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
                    }
                }
                else
                {
                    highlight.SetActive(false);
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

                    if (isMeleeUnit /*&& (agent.remainingDistance < independentPursueDistance)*/)//the latter is how much left in fcurrent patrol, but should always break current patrol to pusue
                    {
                        PursueNearest();
                    }

                }
            }
            else
            {
                deathAnimationTimeElapsed += Time.deltaTime;

                if (health <= 0 && sounds != null && sounds.Length > 0 && !sounds[0].isPlaying && sounds[0].enabled)
                {
                    UnityEngine.Debug.Log("Playing stopped ..");
                    sounds[0].enabled = false;
                    //this.gameObject.SetActive(false);
                }

                if (deathAnimationTimeElapsed > 1f)//length of death animation, which should not be fixed?
                {
                    this.gameObject.SetActive(false);
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
                    animator.speed = 1f;
                    agent.SetDestination(agent.transform.position);
                }
            }
        }

        if (currentTarget)
        {
            //agent.SetDestination(currentTarget.transform.position);
            SetDestinationIfAttainable(agent, currentTarget.transform.position);

            if (agent.remainingDistance < 5)
            {
                if (animator)
                {
                    animator.speed = 3f;
                }
            }
            else
            {
                animator.speed = 1f;
            }
        }
        //else if (!isAwaitingOrders)//else if no current target and no other orders either
        //{
        //    animator.speed = 1f;
        //    agent.SetDestination(agent.transform.position);//if your target is null and you were pursuing nearest then just stop
        //}

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
