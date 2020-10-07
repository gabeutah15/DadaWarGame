using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SelectedUnit
{
    zero = 0,//for the general
    one = 1,
    two,
    three,
    four,
    five,
    six,
    seven,
    eight,
    nine,
    ten,
    eleven,
    twelve,
    thirteen,
    fourteen
}

public class UnitSelectionManager : MonoBehaviour
{
    //needs an entry for each player unit, and a new input in update for each player unit
    //global variable not ideal but if can only select one at a time is fine for prototype i guess
    //public static ;
    public static HashSet<SelectedUnit> selectedUnits;
    int layerMask;
    List<KeyCode> inputKeys;
    bool holdingControl = false;

    // Start is called before the first frame update
    void Start()
    {
        //selectedUnit = SelectedUnit.one;
        layerMask = 1 << 8;//8 is selection layer
        selectedUnits = new HashSet<SelectedUnit>();
        //inputKeys.Add(KeyCode.Alpha0);
        //inputKeys.Add(KeyCode.Alpha1);
        //inputKeys.Add(KeyCode.Alpha2);
        //inputKeys.Add(KeyCode.Alpha3);
        //inputKeys.Add(KeyCode.Alpha4);
        //inputKeys.Add(KeyCode.Alpha5);
        //inputKeys.Add(KeyCode.Alpha6);
        //inputKeys.Add(KeyCode.Alpha7);
        //inputKeys.Add(KeyCode.Alpha8);
        //inputKeys.Add(KeyCode.Alpha9);
        //inputKeys.Add(KeyCode.LeftBracket);
        //inputKeys.Add(KeyCode.RightBracket);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            //Debug.Log("ctrl down");
            holdingControl = true;
        }
        else
        {
            holdingControl = false;
        }

        //this section is maybe not really necessary now but catapults may not have selection planes
        //if(Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    if (selectedUnits.Contains(SelectedUnit.thirteen))
        //    {
        //        selectedUnits.Remove(SelectedUnit.thirteen);
        //    }
        //    else
        //    {
        //        selectedUnits.Add(SelectedUnit.thirteen);
        //    }
        //}
        //else if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    if (selectedUnits.Contains(SelectedUnit.fourteen))
        //    {
        //        selectedUnits.Remove(SelectedUnit.fourteen);
        //    }
        //    else
        //    {
        //        selectedUnits.Add(SelectedUnit.fourteen);
        //    }
        //}

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //{
        //    if (Input.GetKeyDown(KeyCode.LeftControl))
        //    {
        //        selectedUnits.Add(SelectedUnit.one);
        //    }
        //    else
        //    {
        //        selectedUnits.Clear();
        //        selectedUnits.Add(SelectedUnit.one);
        //    }
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha2))
        //{
        //    selectedUnit = SelectedUnit.two;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha3))
        //{
        //    selectedUnit = SelectedUnit.three;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha4))
        //{
        //    selectedUnit = SelectedUnit.four;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha5))
        //{
        //    selectedUnit = SelectedUnit.five;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha6))
        //{
        //    selectedUnit = SelectedUnit.six;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha7))
        //{
        //    selectedUnit = SelectedUnit.seven;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha8))
        //{
        //    selectedUnit = SelectedUnit.eight;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha9))
        //{
        //    selectedUnit = SelectedUnit.nine;
        //}
        //else if (Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    selectedUnit = SelectedUnit.ten;
        //}
        //else if (Input.GetKeyDown(KeyCode.LeftBracket))
        //{
        //    selectedUnit = SelectedUnit.eleven;
        //}
        //else if (Input.GetKeyDown(KeyCode.RightBracket))
        //{
        //    selectedUnit = SelectedUnit.twelve;
        //}

        //this works ok but you have to click the specific unit
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 400, layerMask))
            {
                if (hit.collider.gameObject.GetComponent<AIControl>())
                {
                    AIControl aiControl = hit.collider.gameObject.GetComponent<AIControl>();
                    SelectedUnit selectedUnit = (SelectedUnit)aiControl.selectedUnitNum;

                    //i would like to redo selection bu I'm not sure what should be done
                    //holding control of course does not work on mobile
                    //and single click and drag to move camera gets confused with single touch to select unit
                    //could do simply add a unit if not selected or remove if is when you touch it, ie just:
                    //if(selectedUnits.Contains(selectedUnit))
                    //    {
                    //    selectedUnits.Remove(selectedUnit);
                    //}
                    //    else
                    //{
                    //    selectedUnits.Add(selectedUnit);
                    //}
                    //but then could end up with annoying scenario of wanting to select only one unit and being unable to clear unit selection quickly

                    if (selectedUnit == 0)
                    {
                        selectedUnits.Clear();
                        selectedUnits.Add(selectedUnit);
                    }
                    else if (holdingControl)
                    {
                        selectedUnits.Remove(SelectedUnit.zero);

                        if (selectedUnits.Contains(selectedUnit))
                        {
                            selectedUnits.Remove(selectedUnit);
                        }
                        else
                        {
                            selectedUnits.Add(selectedUnit);
                        }
                    }
                    else
                    {
                        selectedUnits.Remove(SelectedUnit.zero);

                        selectedUnits.Clear();
                        selectedUnits.Add(selectedUnit);
                    }

                    
                }
                else if (hit.collider.gameObject.GetComponentInParent<AIControl>())//selection plane
                {
                    AIControl aiControl = hit.collider.gameObject.GetComponentInParent<AIControl>();
                    SelectedUnit selectedUnit = (SelectedUnit)aiControl.selectedUnitNum;

                    if(selectedUnit == 0)
                    {

                        selectedUnits.Clear();
                        selectedUnits.Add(selectedUnit);
                    }
                    else if (holdingControl)
                    {
                        selectedUnits.Remove(SelectedUnit.zero);

                        //Debug.Log("holding left control");
                        if (selectedUnits.Contains(selectedUnit))
                        {
                            selectedUnits.Remove(selectedUnit);
                        }
                        else
                        {
                            selectedUnits.Add(selectedUnit);
                        }
                    }
                    else
                    {
                        selectedUnits.Remove(SelectedUnit.zero);

                        selectedUnits.Clear();
                        selectedUnits.Add(selectedUnit);
                    }

                }
            }
        }
    }
}
