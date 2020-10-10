using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyBlood : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);//destroyed 2 seconds after instantiated, check performance on this
        //also probably do this for the ground pound effect
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
