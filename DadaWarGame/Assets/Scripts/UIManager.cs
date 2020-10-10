using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public ProgressBar PB;
    public MessagerUIManager MUIM;
    //public GameObject SpawnOBJ;
    //RaycastHit hit;

    // Update is called once per frame
    void Update()
    {
        //update messagerNum
        MUIM.MessagerNum = 4;
        //update progressBar
        PB.current = 50;
        PB.maximum = 100;

        //spawn characters
        //if (Input.GetMouseButtonUp(0))
        //{
        //    if (EventSystem.current.IsPointerOverGameObject())
        //    {
        //        Debug.Log("touch area is UI");
        //    }
        //    else
        //    {
        //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //        if (Physics.Raycast(ray, out hit))
        //        {
        //            Debug.Log("123");
        //            Instantiate(SpawnOBJ, hit.point, transform.rotation);
        //        }
        //    }
        //}

    }
}
