using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    GameObject[] agents;
    int layerMask;
    public float attackASVolume;
    private AudioSource[] audioSourcesNonCatapultAI;
    private AudioSource[] audioSourcesCatapultAI;
    // Start is called before the first frame update
    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("AI");
        layerMask = 1 << 9;//9 is the ground layer mask
        for(int x=0; x< agents.Length; x++)
        {
            if(agents[x].GetComponents<AudioSource>() !=null && (agents[x].GetComponents<AudioSource>()).Length ==2 && audioSourcesNonCatapultAI == null)
            {
                audioSourcesNonCatapultAI = agents[x].GetComponents<AudioSource>();
                audioSourcesNonCatapultAI[1].volume = attackASVolume;
            }
            else if (agents[x].GetComponents<AudioSource>() != null && (agents[x].GetComponents<AudioSource>()).Length == 3 && audioSourcesCatapultAI == null)
            {
                audioSourcesCatapultAI = agents[x].GetComponents<AudioSource>();
                audioSourcesCatapultAI[1].volume = attackASVolume;
            }

            if (audioSourcesCatapultAI != null && audioSourcesNonCatapultAI != null)
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 400, layerMask))
            {
                int unitNum = 0;

                for (int i = 0; i < agents.Length; i++)
                {
                    if (agents[i].activeSelf)//calling deactivate on units once killed, not destroy
                    {
                        if (agents[i].GetComponent<AIControl>())
                        {
                            var aiControl = agents[i].GetComponent<AIControl>();
                            if(/*aiControl.selectedUnitNum == (int)UnitSelectionManager.selectedUnit*/  UnitSelectionManager.selectedUnits.Contains((SelectedUnit)aiControl.selectedUnitNum))
                            {
                                //***this formation movement will be broken by being able to select multiple units at once

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
                                //Plays 'charge prompt' on right click
                                if(agents[i].name.Contains("Catapult"))
                                    audioSourcesCatapultAI[1].Play();
                                else
                                    audioSourcesNonCatapultAI[1].Play();
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
