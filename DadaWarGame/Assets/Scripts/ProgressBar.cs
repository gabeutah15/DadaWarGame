using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public static int maximum;
    public int current;
    public Image mask;

    // Start is called before the first frame update
    void Start()
    {
        //maximum = DeathCounterAndRandomNames.totalEnemies;//total enemies not yet filled out by the time this gappens
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
    }

    void GetCurrentFill()
    {
        float fillAmount = (float)current / (float)maximum;
        mask.fillAmount = fillAmount;
    }
}
