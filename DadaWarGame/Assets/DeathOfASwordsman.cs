using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathOfASwordsman : MonoBehaviour
{
    bool isDead = false;
    float elapsedTimeSinceDeath;
    [SerializeField]
    float timeUntilBodyDisappears = 5;

    private void Awake()
    {
        isDead = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (isDead)
        {
            elapsedTimeSinceDeath += Time.deltaTime;
            if (elapsedTimeSinceDeath > timeUntilBodyDisappears)
            {
                this.gameObject.SetActive(false);
            }
        }
    }
}
