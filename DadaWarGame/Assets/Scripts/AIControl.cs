using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIControl : MonoBehaviour
{
    public NavMeshAgent agent;
    Collider capsuleCollider;
    [SerializeField]
    float health;
    // Start is called before the first frame update
    void Start()
    {
        agent = this.GetComponent<NavMeshAgent>();
        capsuleCollider = this.GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("Projectile"))
        {
            //Debug.Log("hit with arrow");
            health--;
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
        this.transform.LookAt(agent.steeringTarget + new Vector3(0,.5f,0));
        Debug.DrawRay(this.transform.position, agent.steeringTarget + new Vector3(0, .5f, 0));//trying to debug why this lookat makes them flip to the ground when near the target destination
    }

}
