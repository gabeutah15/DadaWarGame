using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddForceTest : MonoBehaviour
{
    Rigidbody rb;
    //[SerializeField]
    //Vector3 targetPosition;
    //[SerializeField]
    public GameObject target;
    [SerializeField]
    float initialAngle;
    bool hasFired = false;
    [HideInInspector]
    public bool isDeadly = true;
    float timerSinceHitting = 0;
    bool timerCounting = false;

    //public AddForceTest()
    //{

    //}
    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();

        //Vector3 thisNoY = this.transform.position - new Vector3(0, this.transform.position.y, 0);
        //Vector3 targetNoY = targetPosition - new Vector3(0, targetPosition.y, 0);
        //var groundDistance = Vector3.Distance(thisNoY, targetNoY);
        //Vector3 toTarget = targetPosition - this.transform.position;
        //rb.AddForce(new Vector3(0, 10, 20), ForceMode.Impulse);
    }

    public void Throw()
    {
        var rigid = GetComponent<Rigidbody>();
        Vector3 p = target.transform.position;
        float gravity = Physics.gravity.magnitude;
        float angle = initialAngle * Mathf.Deg2Rad;
        Vector3 planarTarget = new Vector3(p.x, 0, p.z);
        Vector3 planarPostion = new Vector3(transform.position.x, 0, transform.position.z);
        float distance = Vector3.Distance(planarTarget, planarPostion);
        float yOffset = transform.position.y - p.y;
        float initialVelocity = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));
        Vector3 velocity = new Vector3(0, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));
        float angleBetweenObjects = Vector3.Angle(Vector3.forward, planarTarget - planarPostion);
        Vector3 finalVelocity = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        int randX = Random.Range(-15, 15);
        int randZ = Random.Range(-15, 15);
        Vector3 unitVector = (finalVelocity + new Vector3(randX, 0, randZ)).normalized;
        int rand = Random.Range(-1, 3);
        unitVector *= rand;
        finalVelocity += unitVector;

        rigid.velocity = finalVelocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        LayerMask colliderLayer = collision.collider.gameObject.layer;
        int groundLayer = 1 << 9;
        if (colliderLayer == (colliderLayer | (1 << groundLayer)))
        {
            isDeadly = false;
            timerCounting = true;
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        if (timerCounting)
        {
            timerSinceHitting += Time.deltaTime;
        }

        if(timerSinceHitting > 3)
        {
            this.gameObject.SetActive(false);
        }
    }
}
