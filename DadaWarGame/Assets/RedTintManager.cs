using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedTintManager : MonoBehaviour
{
    public static int totalEnemies;
    public static int numEnemiesDead;
    private UnityEngine.Rendering.PostProcessing.PostProcessVolume volume;
    // Start is called before the first frame update
    void Start()
    {
        volume = gameObject.GetComponent<UnityEngine.Rendering.PostProcessing.PostProcessVolume>();
        volume.weight = 0;
    }

    // Update is called once per frame
    void Update()
    {
        volume.weight = (float)numEnemiesDead / totalEnemies / 2;
    }
}
