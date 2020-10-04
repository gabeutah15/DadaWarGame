using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class UIButtonHandler : MonoBehaviour
{
    GameObject[] playerArchers;
    [SerializeField]
    Button button;
    
    void Start()
    {
        playerArchers = GameObject.FindGameObjectsWithTag("AI");//this just gets all player controlled units
    }

    public void ToggleHoldFire()
    {
        for (int i = 0; i < playerArchers.Length; i++)
        {
            if (playerArchers[i].gameObject.GetComponent<Archer>())
            {
                playerArchers[i].gameObject.GetComponent<Archer>().holdFire = !playerArchers[i].gameObject.GetComponent<Archer>().holdFire;
                if (playerArchers[i].gameObject.GetComponent<Archer>().holdFire)
                {
                    button.GetComponent<Image>().color = Color.red;
                }
                else
                {
                    button.GetComponent<Image>().color = Color.white;
                }
            }
        }
    }
}
