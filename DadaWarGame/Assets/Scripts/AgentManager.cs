﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AgentManager : MonoBehaviour
{
    GameObject[] agents;
    //public static GameObject[] currentlySelectedAgents;//asdf
    [SerializeField]
    LayerMask layerMask;
    //int layerMask;
    public float attackASVolume;
    private AudioSource[] audioSourcesNonCatapultAI;
    private AudioSource[] audioSourcesCatapultAI;
    [SerializeField]
    GeneralOrders general;
    int currentFormationSelectedUnitNum = 0;
    int currentNumFormationsIn = 0;
    public Text numCiviliansSavedText;
    public static int numCiviliansSaved = 0;
    // Start is called before the first frame update
    void Start()
    {

        agents = GameObject.FindGameObjectsWithTag("AI");
        //layerMask = 1 << 9;//9 is the ground layer mask
        for (int x = 0; x < agents.Length; x++)
        {
            if (agents[x].GetComponents<AudioSource>() != null && (agents[x].GetComponents<AudioSource>()).Length == 2 && audioSourcesNonCatapultAI == null)
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

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }


    // Update is called once per frame
    void Update()
    {
        float moralePercent = MoraleManager.GetMoraleMod() + 1;
        moralePercent *= 100;
        numCiviliansSavedText.text = "Civilians Saved: " + numCiviliansSaved.ToString() + " Army Morale: " + moralePercent.ToString("F1") + "%";

        // if (Input.GetMouseButtonDown(1))
        if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject(-1) && !IsPointerOverUIObject())
        {
            RaycastHit hit;
            currentFormationSelectedUnitNum = 0;
            currentNumFormationsIn = 0;
            Vector3 futureDestination = Vector3.zero;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 400, layerMask))
            {
                int unitNum = 0;

                if (!hit.collider.gameObject.GetComponent<AIControl>() && !hit.collider.gameObject.GetComponentInParent<AIControl>())
                {

                    bool hasSentACourier = false;
                    bool isArcherFormation = false;
                    for (int i = 0; i < agents.Length; i++)
                    {
                        

                        if (agents[i].activeSelf)//calling deactivate on units once killed, not destroy
                        {

                            var aiControl = agents[i].GetComponent<AIControl>();

                            if (UnitSelectionManager.selectedUnits.Contains((SelectedUnit)aiControl.selectedUnitNum))
                            {

                                Scout scout = agents[i].GetComponent<Scout>();

                                if (agents[i].GetComponent<GeneralOrders>() || scout)
                                {
                                    if (scout)
                                        scout.IsReturningToGeneral = false;

                                    aiControl.agent.SetDestination(hit.point);
                                }
                                else if (aiControl)//this is probably a nonperformant call
                                {
                                    //if (UnitSelectionManager.selectedUnits.Contains((SelectedUnit)aiControl.selectedUnitNum))
                                    //{

                                    //***this formation movement will be broken by being able to select multiple units at once

                                    //below is really stupid hard coded algo for kind of moving in a formation but only for units of 9
                                    //should be made smarter to account for different unit sizes including changes to size based on loss of units
                                    //this for loop would need to only loop thourhg active units?
                                    //this movement code does not account for narrow tops of walls very well, some units will got behind it if you select to attack on top

                                    if (unitNum == 0)
                                    {
                                        if (agents[i].GetComponent<Archer>())
                                        {
                                            isArcherFormation = true;
                                        }
                                        else
                                        {
                                            isArcherFormation = false;
                                        }
                                    }

                                    unitNum++;
                                    //tried adding some randomization to they don't all cluster on exact destination but doesn't really work
                                    int xPos = unitNum;//Random.Range(-5, 5);

                                    if ((currentFormationSelectedUnitNum != 0) && (currentFormationSelectedUnitNum != aiControl.selectedUnitNum))
                                    {
                                        unitNum = 1;
                                        xPos = 1;
                                        currentNumFormationsIn++;
                                        hasSentACourier = false;
                                        if (agents[i].GetComponent<Archer>())
                                        {
                                            isArcherFormation = true;
                                        }
                                        else
                                        {
                                            isArcherFormation = false;
                                        }
                                    }
                                    currentFormationSelectedUnitNum = aiControl.selectedUnitNum;

                                    //they are having their destination set further and further to the right each time

                                    if (isArcherFormation)
                                    {
                                        xPos += currentNumFormationsIn * 10;
                                    }
                                    else
                                    {
                                        xPos += currentNumFormationsIn * 5;
                                    }

                                    int zPos = 0;// i;//Random.Range(-5, 5);


                                    //being spacing section

                                    if (!isArcherFormation)
                                    {
                                        int shiftValue = 4;

                                        if (unitNum > 4)
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

                                    }
                                    else
                                    {
                                        int shiftValue = 8;

                                        if (unitNum > 8)
                                        {
                                            zPos++;//do another row
                                            xPos -= shiftValue;
                                        }

                                        //if (unitNum > 8)
                                        //{
                                        //    zPos++;//do another row
                                        //    xPos -= shiftValue;
                                        //}

                                        //if (unitNum > 12)
                                        //{
                                        //    zPos++;//do another row
                                        //    xPos -= shiftValue;
                                        //}
                                        xPos -= shiftValue;//center rows
                                    }

                                    //end spacing section
                                    xPos *= 2;
                                    zPos *= 2;

                                    Vector3 destination = hit.point + new Vector3(xPos, 0, zPos);
                                    //Vector3 destination = hit.point;
                                    // agents[i].GetComponent<AIControl>().agent.SetDestination(destination);
                                    //this audio section causing some null ref issues, maybe from units without audiosources
                                    //Plays 'charge prompt' on right click
                                    //if (agents[i].activeSelf)
                                    //{
                                    //if (agents[i].name.Contains("Catapult"))
                                    //{
                                    //    if (audioSourcesCatapultAI[1].isActiveAndEnabled)
                                    //    {
                                    //        audioSourcesCatapultAI[1].Play();
                                    //    }
                                    //}
                                    //else
                                    //{
                                    //    if (audioSourcesNonCatapultAI[1].isActiveAndEnabled)
                                    //    {
                                    //        audioSourcesNonCatapultAI[1].Play();
                                    //    }
                                    //}
                                    // }

                                    if (!hasSentACourier)
                                    {
                                        general.GiveOrder(agents[i].GetComponent<NavMeshAgent>());
                                        hasSentACourier = true;
                                    }
                                    if (hasSentACourier)
                                    {
                                        //hasSentACourier should be true for the whole unit? and is reset to false for next unit?
                                        aiControl.isAwaitingOrders = true;
                                        aiControl.futureDestination = destination;
                                    }
                                    //agents[i].GetComponent<AIControl>().agent.SetDestination(destination);
                                    //simple
                                    //a.GetComponent<AIControl>().agent.SetDestination(hit.point);
                                    // }

                                }

                            }
                        }
                    }
                }
            }
        }
    }
}
