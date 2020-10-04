using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIBody : MonoBehaviour
{
    Collider collider;
    [SerializeField]
    float health = 1;
    Animator animator;
    NavMeshAgent agent;
    GameObject[] playerAgents;
    GameObject currentTarget;
    [SerializeField]
    GameObject deathModelPrefab;
   

    // Start is called before the first frame update
    void Start()
    {
        collider = this.GetComponent<Collider>();
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        playerAgents = GameObject.FindGameObjectsWithTag("AI");
        currentTarget = null;
    }

    private void Update()
    {
        
        if (agent)//basically if it's an enemy that moves, right now not an enemy archer
        {
            
            if (!currentTarget || !currentTarget.activeSelf)
            {
                float minDistance = int.MaxValue;
                GameObject potentialTarget = null;

                for (int i = 0; i < playerAgents.Length; i++)
                {
                    if (playerAgents[i].activeSelf && playerAgents[i].GetComponent<NavMeshAgent>())
                    {
                        float distance = Vector3.Distance(this.transform.position, playerAgents[i].GetComponent<NavMeshAgent>().nextPosition);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            potentialTarget = playerAgents[i];//find closest playeragent that is active and not destroyed
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
                        currentTarget = null;
                    }
                }
            }

            if (currentTarget)
            {
                agent.SetDestination(currentTarget.transform.position);
            }
            if (agent.remainingDistance > 3)
            {
                this.transform.LookAt(agent.steeringTarget + new Vector3(0, .5f, 0));
            }
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
                if(projectileParent.isDeadly){
                    health--;
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
                thisUnitDead.GetComponent<Rigidbody>().AddForce(new Vector3(1f,0,0.5f));//just enough to knock it over
                
            }

            //Destroy(this.gameObject);
        }
    }

}
