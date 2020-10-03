using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AgentManager : MonoBehaviour
{
    GameObject[] agents;

    // Start is called before the first frame update
    void Start()
    {
        agents = GameObject.FindGameObjectsWithTag("AI");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                foreach (GameObject a in agents)
                {
                    if (a.activeSelf)//calling deactivate on units once killed, not destroy
                    {
                        //tried adding some randomization to they don't all cluster on exact destination but doesn't really work
                        //int randomX = Random.Range(-5, 5);
                        //int randomZ = Random.Range(-5, 5);

                        //Vector3 destination = hit.point + new Vector3(hit.point.x + randomX, hit.point.y, hit.point.z + randomZ);

                        //a.GetComponent<AIControl>().agent.SetDestination(destination);

                        a.GetComponent<AIControl>().agent.SetDestination(hit.point);
                        
                    }
                }
            }
        }
    }
}
