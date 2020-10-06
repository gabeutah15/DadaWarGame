using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{
    [HideInInspector]
    public List<GameObject> thisTerritorysDefenders;
    [HideInInspector]
    public List<GameObject> playerUnitsInYourTerritory;
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
            //is getting parent like this because the spear is entering and counting as the other
            //when more enemies enter though we should send trigger to refind targets to enemy ai members of this
            playerUnitsInYourTerritory.Add(other.gameObject.GetComponentInParent<AIControl>().gameObject);//aa
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.GetComponentInParent<AIControl>())
        {
            playerUnitsInYourTerritory.Remove(other.gameObject.GetComponentInParent<AIControl>().gameObject);//aa
        }
    }



}
