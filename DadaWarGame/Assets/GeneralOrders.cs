using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GeneralOrders : MonoBehaviour
{


    [SerializeField]
    GameObject courierPrefab;//make into array of limited size later if this is too annoying to only have one
    List<Courier> couriers;
    //Dictionary<NavMeshAgent, NavMeshAgent> courierDictionary;
    NavMeshAgent generalsAgent;
    GameObject[] agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("AI");
        couriers = new List<Courier>();
        //for (int i = 0; i < 5; i++)
        //{
        //    Courier newCourier = new Courier();//do this later with fixed number of couriers
        //}
        //courierDictionary = new Dictionary<NavMeshAgent, NavMeshAgent>();
        generalsAgent = this.GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < couriers.Count; i++)
        {
            //Courier thisCourier = couriers[i];//this ends up being a value not ref?
            couriers[i].agent.SetDestination(couriers[i].targetsNavMeshAgent.nextPosition);
            if (couriers[i].agent.pathPending)
                continue;

            if (couriers[i].agent.remainingDistance < 5)
            {
                //arriving at unit destination, so set to the general
                if (couriers[i].target != this.gameObject)
                {
                    for (int j = 0; j < agents.Length; j++)
                    {
                        var aiControl = agents[j].GetComponent<AIControl>();

                        if (aiControl && agents[j].activeSelf)//this is probably a nonperformant call
                        {
                            if(aiControl.selectedUnitNum == couriers[i].targetUnitNum)//this never goes off, why?
                            {
                                aiControl.isAwaitingOrders = false;
                                if (aiControl.agent)
                                {
                                    aiControl.agent.SetDestination(aiControl.futureDestination);
                                }
                            }

                        }
                    }

                    couriers[i].target = this.gameObject;
                    couriers[i].targetsNavMeshAgent = this.generalsAgent;
                }
                else//arriving at the general, so turn off? or make available again for more missions?
                {
                    //thisCourier.available = true;//do later with fixed num couriers
                    couriers[i].gameObject.SetActive(false);
                    couriers.Remove(couriers[i]);
                }
            }

        }

        //foreach (KeyValuePair<NavMeshAgent,NavMeshAgent> entry in courierDictionary)
        //{
        //    if(entry.Key.remainingDistance < 2)
        //    {
        //        if(entry.Value != generalsAgent)
        //        {
        //            courierDictionary[entry.Key] = generalsAgent;//change back to general if you get to target
        //        }
        //        else
        //        {
        //            GameObject courierToRemove = entry.Key.gameObject;
        //            //courierDictionary.Remove(entry.Key);//you've reached the general
        //            //courierToRemove.SetActive(false);
        //        }
        //    }

        //    entry.Key.SetDestination(entry.Value.nextPosition);//next or this position?
        //}
    }

    public void GiveOrder(NavMeshAgent targetAgent)
    {
        if (this.gameObject.activeSelf)
        {
            GameObject thisCourier = Instantiate(courierPrefab, this.transform.position + new Vector3(0, 0, 2), Quaternion.identity) as GameObject;
            //couriers.Add(thisCourier);
            NavMeshAgent thisCouriersAgent = thisCourier.GetComponent<NavMeshAgent>();
            Courier courier = thisCourier.GetComponent<Courier>();
            courier.agent = thisCouriersAgent;
            courier.target = targetAgent.gameObject;
            courier.targetsNavMeshAgent = targetAgent;
            courier.targetUnitNum = targetAgent.gameObject.GetComponent<AIControl>().selectedUnitNum;
            //thisCouriersAgent.SetDestination(targetAgent.transform.position);//does this always update?
            //courier.agent.SetDestination(targetAgent.gameObject.transform.position);//set to current position not next navagent postion becaus target might not have next pos yet
            couriers.Add(courier);
            //courierDictionary.Add(thisCouriersAgent, targetAgent);
        }
    }
}
