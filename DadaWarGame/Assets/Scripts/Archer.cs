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
    GameObject currentTarget;
    float timer;
    float targetDistance;
    [SerializeField]
    string tagToShoot;

    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag(tagToShoot);

        if(tagToShoot == "EnemyAI")
        {
            var a = 5;
        }
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 2)
        {
            timer -= 2;
            if (currentTarget && currentTarget.activeSelf)
            {
                ShootProjectile(currentTarget);
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
                var test = 1;
                if (targets[i].GetComponent<Archer>() && !targets[i].GetComponent<AIControl>())
                {
                    test = 5;
                    //it is going in here and finding archer targets but arrows just fall to floor, BUT only when shooting at archers
                }

                if (targets[i] && targets[i].activeSelf/* && targets[i].GetComponent<NavMeshAgent>()*/)
                {
                    //added this section because some ai enemy targerts have no navmesh, don't move, in which case just shoot at their position
                    Vector3 targetPos = targets[i].transform.position;
                    if (targets[i].GetComponent<NavMeshAgent>())
                    {
                        targetPos = targets[i].GetComponent<NavMeshAgent>().nextPosition;
                    }
                    float distance = Vector3.Distance(this.transform.position, targetPos);

                    //this could be changed to make them pick a kind of random target within range rather than just the first one, because right now
                    //all of the archers end up shooting at the same target and then switching to a new target
                    if (distance < range)
                    {
                        //this randomization actually works decently to make targets more random and not have all the units shoot at the same target
                        int rand = Random.Range(0, 4);
                        if(rand == 0)
                        {
                            currentTarget = targets[i];
                            targetDistance = distance;
                        }
                    }
                }
            }
        }
        else
        {
            //similar added for non mesh agent enemies
            Vector3 targetPos = currentTarget.transform.position;
            if (currentTarget.GetComponent<NavMeshAgent>())
            {
                targetPos = currentTarget.GetComponent<NavMeshAgent>().nextPosition;
            }

            float distance = Vector3.Distance(this.transform.position, targetPos);
            if (distance > range)
            {
                currentTarget = null;
            }
        }
    }

    public void ShootProjectile(GameObject targetUnit)
    {
        Transform p = AmmoManager.SpawnAmmo(this.transform.position, Quaternion.identity);

        //similar added for non mesh agent enemies
        Vector3 targetPos = targetUnit.transform.position;
        if (targetUnit.GetComponent<NavMeshAgent>())
        {
            targetPos = targetUnit.GetComponent<NavMeshAgent>().nextPosition;
            if (targetUnit.GetComponent<NavMeshAgent>().velocity.sqrMagnitude < .1f)
            {
                /*targetPosition*/ targetPos = targetUnit.transform.position;
            }
        }

        //Vector3 targetPosition = targetUnit.GetComponent<NavMeshAgent>().nextPosition;

        p.GetComponent<Projectile>().Initialize(/*targetPosition*/targetPos, targetUnit.GetComponent<NavMeshAgent>(), targetDistance);
    }
}

