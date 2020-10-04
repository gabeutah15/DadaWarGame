using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    GameObject[] agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("AI");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 400))
            {
                int unitNum = 0;

                for (int i = 0; i < agents.Length; i++)
                {
                    if (agents[i].activeSelf)//calling deactivate on units once killed, not destroy
                    {
                        if (agents[i].GetComponent<AIControl>())
                        {
                            var aiControl = agents[i].GetComponent<AIControl>();
                            if(aiControl.selectedUnitNum == (int)UnitSelectionManager.selectedUnit)
                            {

                                //below is really stupid hard coded algo for kind of moving in a formation but only for units of 9
                                //should be made smarter to account for different unit sizes including changes to size based on loss of units
                                //this for loop would need to only loop thourhg active units?
                                //this movement code does not account for narrow tops of walls very well, some units will got behind it if you select to attack on top
                                unitNum++;
                                //tried adding some randomization to they don't all cluster on exact destination but doesn't really work
                                int randomX = unitNum;//Random.Range(-5, 5);
                                int randomZ = 0;// i;//Random.Range(-5, 5);

                                if(unitNum > 3)
                                {
                                    randomZ++;//do another row
                                    randomX -= 4;
                                }
                                if (unitNum > 6)
                                {
                                    randomZ++;//do another row
                                    randomX -= 4;
                                }
                                randomX -= 2;//center rows of 4
                                randomX *= 2;
                                randomZ *= 4;

                                Vector3 destination = hit.point + new Vector3(randomX, 0, randomZ);
                                //Vector3 destination = hit.point;


                                agents[i].GetComponent<AIControl>().agent.SetDestination(destination);

                                //simple
                                //a.GetComponent<AIControl>().agent.SetDestination(hit.point);
                            }

                        }
                        
                    }

                }
            }
        }
    }
}
