using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoraleManager : MonoBehaviour
{
    public static int numDeathsForMaxDebuff = 100;
    public static int numCivsForMaxBuff = 5;
    public static int numDeaths = 0;

    public static float GetMoraleMod()
    {
        return ((float)AgentManager.numCiviliansSaved / numCivsForMaxBuff) - ((float)numDeaths / numDeathsForMaxDebuff); 
    }
}
