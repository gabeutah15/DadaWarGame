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
    
    Animator animator;
    private int leftAnimHash;
    private int rightAnimHash;
    private int forwardAnimHash;
    private int backwardAnimHash;
    private int isMovingHash;
    private int isDeadHash;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        leftAnimHash = Animator.StringToHash("Left2");
        rightAnimHash = Animator.StringToHash("Right2");
        forwardAnimHash = Animator.StringToHash("Forward2");
        backwardAnimHash = Animator.StringToHash("Backward2");
        isMovingHash = Animator.StringToHash("IsMoving");
        isDeadHash = Animator.StringToHash("Death");

        if (animator)
        {
            animator.speed = 1f;
        }
    }

    public void GoToDestination(int index)
    {
        agent.SetDestination(wayPoints[index].position);
    }

    // Update is called once per frame
    void Update()
    {
        

        if (agent.remainingDistance < 4)
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

    private void LateUpdate()
    {
        if (animator)
        {

            if (agent.velocity.sqrMagnitude > 0.1f)
            {
                animator.SetBool(isMovingHash, true);
            }
            else
            {
                animator.SetBool(isMovingHash, false);
            }

            Vector3 rotation = (this.transform.rotation * Vector3.forward).normalized;
            float x = rotation.x;
            float z = rotation.z;
            if (Mathf.Abs(x) > Mathf.Abs(z))
            {
                if (x < 0)
                {
                    //RIGHT
                    animator.SetBool(leftAnimHash, false);
                    animator.SetBool(rightAnimHash, true);
                    animator.SetBool(forwardAnimHash, false);
                    animator.SetBool(backwardAnimHash, false);
                }
                else
                {
                    //LEFT
                    animator.SetBool(leftAnimHash, true);
                    animator.SetBool(rightAnimHash, false);
                    animator.SetBool(forwardAnimHash, false);
                    animator.SetBool(backwardAnimHash, false);
                }
            }
            else
            {
                //more forward or back
                if (z < 0)
                {
                    //Backward
                    animator.SetBool(leftAnimHash, false);
                    animator.SetBool(rightAnimHash, false);
                    animator.SetBool(forwardAnimHash, false);
                    animator.SetBool(backwardAnimHash, true);
                }
                else
                {
                    //Forward
                    animator.SetBool(leftAnimHash, false);
                    animator.SetBool(rightAnimHash, false);
                    animator.SetBool(forwardAnimHash, true);
                    animator.SetBool(backwardAnimHash, false);
                }
            }

        }
    }
}
