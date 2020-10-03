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

    void Start()
    {
        targets = GameObject.FindGameObjectsWithTag("AI");
    }

    private void FixedUpdate()
    {
        timer += Time.deltaTime;
        if (timer > 2)
        {
            timer -= 2;
            if (currentTarget)
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
                if (targets[i].activeSelf && targets[i].GetComponent<NavMeshAgent>())
                {
                    float distance = Vector3.Distance(this.transform.position, targets[i].GetComponent<NavMeshAgent>().nextPosition);

                    if (distance < range)
                    {
                        currentTarget = targets[i];
                        targetDistance = distance;
                    }
                }
            }
        }
        else
        {
            float distance = Vector3.Distance(this.transform.position, currentTarget.GetComponent<NavMeshAgent>().nextPosition);
            if (distance > range)
            {
                currentTarget = null;
            }
        }
    }

    public void ShootProjectile(GameObject targetUnit)
    {
        Transform p = AmmoManager.SpawnAmmo(this.transform.position, Quaternion.identity);
        p.GetComponent<Projectile>().Initialize(targetUnit.GetComponent<NavMeshAgent>().nextPosition, targetUnit.GetComponent<NavMeshAgent>(), targetDistance);
    }
}

