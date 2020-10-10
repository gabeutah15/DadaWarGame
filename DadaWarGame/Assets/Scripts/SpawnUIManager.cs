using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpawnUIManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{

    private float delay = 0.1f;
    private bool isDown = false;

    private float lastIsDownTime;


    public GameObject _SpawnObj;

    void Update()
    {
        if (isDown)
        {
            if (Time.time - lastIsDownTime > delay)
            {
                // do things when long press
                Debug.Log("long press");
                lastIsDownTime = Time.time;
            }
        }

    }

    //do things when press down the key
    public void OnPointerDown(PointerEventData eventData)
    {
        isDown = true;
        lastIsDownTime = Time.time;
        
    }

    //do things when release the mouse
    public void OnPointerUp(PointerEventData eventData)
    {
        isDown = false;
       

    }
    // do things when mouse leaves the button
    public void OnPointerExit(PointerEventData eventData)
    {
        isDown = false;
    }
}
