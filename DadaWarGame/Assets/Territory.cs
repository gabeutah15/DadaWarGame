using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> thisTerritorysDefenders;
    [HideInInspector]
    public List<GameObject> thisTerritorysPlayerUnitsThatHaveEntered;
    // Start is called before the first frame update
    void Start()
    {
        thisTerritorysDefenders = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponentInParent<AIControl>())
        {
            thisTerritorysPlayerUnitsThatHaveEntered.Add(other.gameObject.GetComponentInParent<AIControl>().gameObject);//aa
        }
    }


    
}
