using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class ScoutManager : MonoBehaviour
{
    //GameObject[] initialScouts;
    List<GameObject> scouts;
    [SerializeField]
    GameObject scoutsPrefab;
    GeneralOrders general;
    int numInitialScouts = 5;//***this is only incidentally the same as number of couriers, but they should be set to the same variable that is constant
    // Start is called before the first frame update
    [HideInInspector]
    public static int numCurrentScouts = 0;
    public Text numCurrentScoutsDisplay;
    bool hasSetInitialScoutsFalse = false;

    void Awake()
    {
        general = FindObjectOfType<GeneralOrders>();//should only be one, could alternatively expose this as serialized field
        scouts = new List<GameObject>();
        for (int i = 0; i < numInitialScouts; i++)
        {
            GameObject thisScout = Instantiate(scoutsPrefab, general.gameObject.transform.position + new Vector3(2, -1, 4),Quaternion.identity);
            thisScout.GetComponent<AIControl>().selectedUnitNum = 21 + i;//21,22,23,24,25 RESERVED for scouts
            //thisScout.SetActive(false);
            scouts.Add(thisScout);
        }
    }

    public void SpawnScout()//***this fails if you try to spam spawn scouts while MAX couriers are out, ie while there aren't enough couriers to spawn a scout
    {
        //need to check here on num available couriers, or only be able to call this if num available couriers is greater than 0
        //that can be done in the onclick ui for the button
        if(GeneralOrders.couriersCurrentlyAvailable > 0)
        {
            int spawnIndex = 0;
            GameObject scoutToSpawn = null;
            if (numCurrentScouts < numInitialScouts)
            {
                for (int i = 0; i < scouts.Count; i++)
                {
                    if (!scouts[i].activeSelf)
                    {
                        scoutToSpawn = scouts[i];
                        spawnIndex = i;
                        break;
                    }
                }
            }

            if (scoutToSpawn)
            {
                numCurrentScouts++;
                GeneralOrders.couriersCurrentlyAvailable--;
                scoutToSpawn.SetActive(true);
                scoutToSpawn.transform.position = general.gameObject.transform.position + new Vector3(4 + spawnIndex * 4, 0, 6);
                scoutToSpawn.GetComponent<NavMeshAgent>().SetDestination(scoutToSpawn.transform.position);
            }
        }
    }

    //this would be called on a button called ReturnScout, which would only be active if numCurrentScouts > 0
    //and should only be able to select one scout at a time so know which one you are calling this on (the one whose number is the selected unit numer)
    public void ReturnScout()
    {
        for (int i = 0; i < scouts.Count; i++)
        {
            if (UnitSelectionManager.selectedUnits.Contains((SelectedUnit)scouts[i].GetComponent<AIControl>().selectedUnitNum))
            {
                //**note that should only be permitted to select one scout at a time I think? or don't bother with that and be able to return multiple at once?
                //i guess i'll do the latter for now
                scouts[i].GetComponent<NavMeshAgent>().SetDestination(general.transform.position);
                scouts[i].GetComponent<Scout>().IsReturningToGeneral = true;
                //only subtract from available if it reaches general, you should be able to waylay it if otherwise, BUT if so then when giving an order to a scout 
                //you need to set it's bool of IsReturningToGeneral to false;
            }
        }
    }

    public void ConsumeScout(GameObject scout)
    {
        numCurrentScouts--;
        GeneralOrders.couriersCurrentlyAvailable++;
        //Call to increase available couriers
        scout.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasSetInitialScoutsFalse)
        {
            for (int i = 0; i < scouts.Count; i++)
            {
                scouts[i].SetActive(false);
            }
            hasSetInitialScoutsFalse = true;
        }

        numCurrentScoutsDisplay.text = "Num Current Scouts: " + numCurrentScouts.ToString();
    }
}
