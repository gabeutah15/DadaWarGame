using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    GameObject[] agents;
    //public static GameObject[] currentlySelectedAgents;
    int layerMask;

    // Start is called before the first frame update
    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("AI");
        layerMask = 1 << 9;//9 is the ground layer mask
    }

    int currentFormationSelectedUnitNum = 0;
    int currentNumFormationsIn = 0;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            currentFormationSelectedUnitNum = 0;
            currentNumFormationsIn = 0;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 400, layerMask))
            {
                int unitNum = 0;

                for (int i = 0; i < agents.Length; i++)
                {
                    if (agents[i].activeSelf)//calling deactivate on units once killed, not destroy
                    {

                        var aiControl = agents[i].GetComponent<AIControl>();

                        if (aiControl)//this is probably a nonperformant call
                        {
                            
                            if(UnitSelectionManager.selectedUnits.Contains((SelectedUnit)aiControl.selectedUnitNum))
                            {

                                //***this formation movement will be broken by being able to select multiple units at once

                                //below is really stupid hard coded algo for kind of moving in a formation but only for units of 9
                                //should be made smarter to account for different unit sizes including changes to size based on loss of units
                                //this for loop would need to only loop thourhg active units?
                                //this movement code does not account for narrow tops of walls very well, some units will got behind it if you select to attack on top
                                unitNum++;
                                //tried adding some randomization to they don't all cluster on exact destination but doesn't really work
                                int xPos = unitNum;//Random.Range(-5, 5);

                                if((currentFormationSelectedUnitNum != 0) && (currentFormationSelectedUnitNum != aiControl.selectedUnitNum))
                                {
                                    unitNum = 1;
                                    xPos = 1;
                                    currentNumFormationsIn++;
                                }
                                currentFormationSelectedUnitNum = aiControl.selectedUnitNum;

                                //they are having their destination set further and further to the right each time

                                xPos += currentNumFormationsIn * 5;

                                int zPos = 0;// i;//Random.Range(-5, 5);

                                int shiftValue = 4;

                                if(unitNum > 4)
                                {
                                    zPos++;//do another row
                                    xPos -= shiftValue;
                                }

                                if (unitNum > 8)
                                {
                                    zPos++;//do another row
                                    xPos -= shiftValue;
                                }

                                if (unitNum > 12)
                                {
                                    zPos++;//do another row
                                    xPos -= shiftValue;
                                }
                                xPos -= shiftValue;//center rows of 4
                                xPos *= 2;
                                zPos *= 4;

                                Vector3 destination = hit.point + new Vector3(xPos, 0, zPos);
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
