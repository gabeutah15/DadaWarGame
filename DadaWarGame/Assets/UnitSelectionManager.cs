using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    //needs an entry for each player unit, and a new input in update for each player unit
    public enum SelectedUnit
    {
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
        twelve
    }

    //global variable not ideal but if can only select one at a time is fine for prototype i guess
    public static SelectedUnit selectedUnit;

    // Start is called before the first frame update
    void Start()
    {
        selectedUnit = SelectedUnit.one;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Debug.Log("pressed one");
            selectedUnit = SelectedUnit.one;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedUnit = SelectedUnit.two;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedUnit = SelectedUnit.three;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedUnit = SelectedUnit.four;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            selectedUnit = SelectedUnit.five;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            selectedUnit = SelectedUnit.six;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            selectedUnit = SelectedUnit.seven;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            selectedUnit = SelectedUnit.eight;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            selectedUnit = SelectedUnit.nine;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            selectedUnit = SelectedUnit.ten;
        }
        else if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            selectedUnit = SelectedUnit.eleven;
        }
        else if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            selectedUnit = SelectedUnit.twelve;
        }
    }
}
