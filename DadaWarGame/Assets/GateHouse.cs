using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GateHouse : MonoBehaviour
{
    Gate gate;
    GateDestroyed gateDestroyed;

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
        Debug.Log("wall collided");
        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectileParent = collision.gameObject.GetComponentInParent<Projectile>();
            if (projectileParent)
            {
                //if (projectileParent.isDeadly)
                //{
                gateDestroyed.gameObject.SetActive(true);
                gate.gameObject.SetActive(false);
                //apply some force here or will it kind of fall apart on its own?
                //}
            }
        }
    }

}
