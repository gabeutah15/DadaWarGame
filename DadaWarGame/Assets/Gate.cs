using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnCollisionEnter(Collision collision)
    //{
        
    //}

    void OnCollisionEnter(Collision collision)
    {
        transform.parent.GetComponent<GateHouse>().CollisionDetected(this, collision);
    }
}
