using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GeneralOrders : MonoBehaviour
{


    [SerializeField]
    GameObject courierPrefab;//make into array of limited size later if this is too annoying to only have one
    List<Courier> couriers;
    //Dictionary<NavMeshAgent, NavMeshAgent> courierDictionary;
    NavMeshAgent generalsAgent;
    GameObject[] agents;
    [SerializeField]
    private int totalStartingCouriers = 5;
    [HideInInspector]
    public static int couriersCurrentlyAvailable = 5;
    Queue<NavMeshAgent> agentsAwaitingOrders;
    public Text numCouriersAvailable;

    // Start is called before the first frame update
    void Start()
    {
        

        agentsAwaitingOrders = new Queue<NavMeshAgent>();
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
        numCouriersAvailable.text = "Couriers Available: " + couriersCurrentlyAvailable.ToString();

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
                    couriersCurrentlyAvailable++;
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

        if ((couriersCurrentlyAvailable > 0) && (agentsAwaitingOrders.Count > 0))
        {
            NavMeshAgent thisTargetAgnet = agentsAwaitingOrders.Dequeue();
            couriersCurrentlyAvailable--;
            GameObject thisCourier = Instantiate(courierPrefab, this.transform.position + new Vector3(0, 0, 2), Quaternion.identity) as GameObject;
            //couriers.Add(thisCourier);
            NavMeshAgent thisCouriersAgent = thisCourier.GetComponent<NavMeshAgent>();
            Courier courier = thisCourier.GetComponent<Courier>();
            courier.agent = thisCouriersAgent;
            courier.target = thisTargetAgnet.gameObject;
            courier.targetsNavMeshAgent = thisTargetAgnet;
            courier.targetUnitNum = thisTargetAgnet.gameObject.GetComponent<AIControl>().selectedUnitNum;
            couriers.Add(courier);
        }
    }

    public void GiveOrder(NavMeshAgent targetAgent)
    {
        if (this.gameObject.activeSelf)
        {
            if((couriersCurrentlyAvailable > 0) && (agentsAwaitingOrders.Count == 0))//couriers available and no one waiting to get one
            {
                couriersCurrentlyAvailable--;
                GameObject thisCourier = Instantiate(courierPrefab, this.transform.position + new Vector3(0, 0, 2), Quaternion.identity) as GameObject;
                //couriers.Add(thisCourier);
                NavMeshAgent thisCouriersAgent = thisCourier.GetComponent<NavMeshAgent>();
                Courier courier = thisCourier.GetComponent<Courier>();
                courier.agent = thisCouriersAgent;
                courier.target = targetAgent.gameObject;
                courier.targetsNavMeshAgent = targetAgent;
                courier.targetUnitNum = targetAgent.gameObject.GetComponent<AIControl>().selectedUnitNum;
                couriers.Add(courier);
            }
            else
            {
                agentsAwaitingOrders.Enqueue(targetAgent);
            }
        }
    }
   
}
