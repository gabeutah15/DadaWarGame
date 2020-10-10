using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MessagerUIManager : MonoBehaviour
{
    public Image M1;
    public Image M2;
    public Image M3;
    public Image M4;
    public Image M5;
    public int MessagerNum;


    // Update is called once per frame
    void Update()
    {
        if (MessagerNum == 5)
        {
            M1.gameObject.SetActive(true);
            M2.gameObject.SetActive(true);
            M3.gameObject.SetActive(true);
            M4.gameObject.SetActive(true);
            M5.gameObject.SetActive(true);
        }
        if (MessagerNum == 4)
        {
            M1.gameObject.SetActive(true);
            M2.gameObject.SetActive(true);
            M3.gameObject.SetActive(true);
            M4.gameObject.SetActive(true);
            M5.gameObject.SetActive(false);
        }
        if (MessagerNum == 3)
        {
            M1.gameObject.SetActive(true);
            M2.gameObject.SetActive(true);
            M3.gameObject.SetActive(true);
            M4.gameObject.SetActive(false);
            M5.gameObject.SetActive(false);
        }
        if (MessagerNum == 2)
        {
            M1.gameObject.SetActive(true);
            M2.gameObject.SetActive(true);
            M3.gameObject.SetActive(false);
            M4.gameObject.SetActive(false);
            M5.gameObject.SetActive(false);
        }
        if (MessagerNum == 1)
        {
            M1.gameObject.SetActive(true);
            M2.gameObject.SetActive(false);
            M3.gameObject.SetActive(false);
            M4.gameObject.SetActive(false);
            M5.gameObject.SetActive(false);
        }
        if (MessagerNum == 0)
        {
            M1.gameObject.SetActive(false);
            M2.gameObject.SetActive(false);
            M3.gameObject.SetActive(false);
            M4.gameObject.SetActive(false);
            M5.gameObject.SetActive(false);
        }


    }
}
