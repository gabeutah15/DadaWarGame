using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

//***this is just the civilian script now
public class FollowPath : MonoBehaviour
{
    [SerializeField]
    Transform[] wayPoints;
    NavMeshAgent agent;
    int destinationIndex;
    [SerializeField]
    GameObject disappearParticle;
    private float timeSinceParticleElapsed;
    private bool isRescued = false;
    public bool numCiviliansSaved;
    

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

    }

    public void GoToDestination(int index)
    {
        agent.SetDestination(wayPoints[index].position);
    }

    // Update is called once per frame
    void Update()
    {
        

        if (agent.remainingDistance < 0.1)
        {
            destinationIndex++;
            destinationIndex = destinationIndex % wayPoints.Length;
            GoToDestination(destinationIndex);
        }

        if (isRescued)
        {
            timeSinceParticleElapsed += Time.deltaTime;

            if(timeSinceParticleElapsed > 2f)
            {
                Destroy(disappearParticle);
                this.gameObject.SetActive(false);
            }
        }

       
    }

    private void OnCollisionEnter(Collision collision)
    {
        var parent = collision.gameObject.GetComponentInParent<AIControl>();
        if (parent)
        {
            AgentManager.numCiviliansSaved++;
            Instantiate(disappearParticle, transform.position + new Vector3(0, 0, 0), Quaternion.identity);
            isRescued = true;
            agent.speed = 0;
            agent.velocity = new Vector3(0,0,0);

        }
    }
}
