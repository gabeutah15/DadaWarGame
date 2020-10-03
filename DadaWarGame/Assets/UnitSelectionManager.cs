using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    public enum SelectedUnit
    {
        one = 1,
        two,
        three
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
            Debug.Log("pressed one");
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
    }
}
