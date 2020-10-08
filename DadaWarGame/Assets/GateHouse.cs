using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.AI;


public class GateHouse : MonoBehaviour
{
    Gate gate;
    GateDestroyed gateDestroyed;
    [SerializeField]
    int gateHealth = 5;

    // Start is called before the first frame update
    void Start()
    {
        gate = GetComponentInChildren<Gate>();
        gate.gameObject.SetActive(true);
        gateDestroyed = GetComponentInChildren<GateDestroyed>();
        gateDestroyed.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CollisionDetected(Gate gate, Collision collision)
    {
        //Debug.Log("wall collided");
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectileParent = collision.gameObject.GetComponentInParent<Projectile>();
            if (projectileParent)
            {
                gateHealth--;
                //if (projectileParent.isDeadly)
                //{
                
                //rebuild nav mesh after destroying wall so you can walk through it
                //apply some force here or will it kind of fall apart on its own?
                //}
            }
        }

        AddForceTest physicsBall = collision.gameObject.GetComponent<AddForceTest>();
        if (physicsBall)
        {
            if (physicsBall.isDeadly)
            {
                gateHealth -= 2;
            }
        }

        if (gateHealth <= 0)
        {
            //TargetsManager.RemoveFromTargetsList(gate.gameObject);//this might not be right, not sure if this alone is what is being targetted ***
            gateDestroyed.gameObject.SetActive(true);
            gate.gameObject.SetActive(false);
            //should do something to remove this from targets list once destroyed so catapult doesn't keep shooting it
            //also maybe something to prevent arrows from shooting at gates
            //NavMeshBuilder.BuildNavMesh();//this might be editor only?
        }
    }

}
