using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Projectile : MonoBehaviour
{
    public Vector3 target;
    public float highFiringAngle = 45.0f;
    public float lowFiringAngle = 7.0f;
    public float directFireRange = 35;
    //public float loftedFireRange = 150;
    private float currentFiringAngle;

    public float speed = 10f;

    public bool isDeadly = false;//only become deadly after say one second so doesn't kill archer that shoots it, and so has a kind of min range

    public Transform arrowModel;
    [HideInInspector]
    public float elapsedTime;
    public float flightDuration { get; private set; }

    Vector2 velocity;
    Vector3 lastPosition;

    public bool enableRandomTargetArea = false;
    public float randomRadius = 2f;
    public bool enableRandomSpeed = false;
    public float randomSpeedMin = 8f;
    public float randomSpeedMax = 20f;
    public bool IsLeadBall;

    public bool isFlying;
    public float visibleTime = 5;
    public AudioSource impactSound;
    private int collisionCount = 0;

    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponentInChildren<Rigidbody>();
    }

    public void Initialize(Vector3 _target, NavMeshAgent _targetNavMeshAgent, float distanceToTarget)
    {

        if (distanceToTarget < directFireRange)
        {
            currentFiringAngle = lowFiringAngle;
        }
        else
        {
            currentFiringAngle = highFiringAngle;
        }

        isFlying = true;

        if (enableRandomTargetArea)
        {
            Vector2 randomCircle = Random.insideUnitCircle * randomRadius;
            Vector3 finalCircle = new Vector3(randomCircle.x, 0, randomCircle.y);
            target = _target + finalCircle;
        }

        if (enableRandomSpeed) speed = Random.Range(randomSpeedMin, randomSpeedMax);

        const int predictIterations = 5;
        Vector3 predictTarget = target;
        for (int i = 0; i < predictIterations; i++)
        {
            //added some extra direction to account for arrows falling short
            Vector3 extraDirection = predictTarget - transform.position;
            extraDirection = extraDirection.normalized;
            int addedScalar = 7;
            if (IsLeadBall)
                addedScalar = 17;
            extraDirection *= addedScalar;
            //calculate distance to target
            predictTarget += extraDirection;
            float dist = Vector3.Distance(transform.position, predictTarget);
            float vel = Mathf.Sqrt((dist * speed) / Mathf.Sin(2 * currentFiringAngle * Mathf.Deg2Rad));

            velocity.x = vel * Mathf.Cos(currentFiringAngle * Mathf.Deg2Rad);
            velocity.y = vel * Mathf.Sin(currentFiringAngle * Mathf.Deg2Rad);

            //calculate flight time
            flightDuration = dist / velocity.x;

            //HAVE NON NAVMESHAGENT TARGETS NOW
            if (_targetNavMeshAgent)
            {
                predictTarget = target + _targetNavMeshAgent.velocity * flightDuration;
            }
            else
            {
                predictTarget = target;
            }
        }

        //rotate projectile to face the target
        Vector3 dir = predictTarget - transform.position;
        if (dir != Vector3.zero) transform.rotation = Quaternion.LookRotation(dir);

        elapsedTime = 0;

        lastPosition = transform.position;
    }
    private void Start()
    {
        impactSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if ((elapsedTime > 0.1f) && isFlying && gameObject.activeSelf)
        {
            isDeadly = true;
        }

        if (!isFlying)
        {
            isDeadly = false;
        }

        //if(elapsedTime > .2f)
        //{
        //    isFlying = false;
        //}
        //if (elapsedTime > flightDuration)
        //{
        //    if (rb && IsLeadBall)
        //    {
        //        isFlying = false;
        //        rb.AddForce(new Vector3(0, 10 * velocity.y, 10 * velocity.x));
        //    }
        //}

        if (elapsedTime < flightDuration && isFlying)
        {
            //translate the x component on the z-axis => forward vector (rotiation)
            transform.Translate(0, (velocity.y - (speed * elapsedTime)) * Time.deltaTime, velocity.x * Time.deltaTime);
        }
        else if (elapsedTime > flightDuration + visibleTime)
        {
            //deactivate obj
            gameObject.SetActive(false);
        }

        elapsedTime += Time.deltaTime;
        Vector3 dir = transform.position - lastPosition;
        if (dir != Vector3.zero) arrowModel.rotation = Quaternion.LookRotation(dir);
        lastPosition = transform.position;
    }

     private void OnCollisionEnter(Collision collision)
    {
        
        if (IsLeadBall)
        {   
            if (collision.gameObject.name.Equals("Plane") && collisionCount ==0)
            {   
                collisionCount++;
                impactSound.Play();
            }
        }
    }

}