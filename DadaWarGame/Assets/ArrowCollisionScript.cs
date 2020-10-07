using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollisionScript : MonoBehaviour
{
    Projectile projectileParent;
    Rigidbody rb;
    [SerializeField]
    ParticleSystem groundPoundPS;
    bool particleHasPlayed = false;
    float timeSinceImpact = 0;
    AddForceTest force;
    // Start is called before the first frame update
    void Start()
    {
        projectileParent = GetComponentInParent<Projectile>();//sometimes this is null but it shouldn't be?
        rb = GetComponent<Rigidbody>();
        force = GetComponent<AddForceTest>();
    }

    private void Update()
    {
        if (particleHasPlayed)
        {
            timeSinceImpact += Time.deltaTime;
        }

        if(timeSinceImpact > 2)
        {
            //groundPoundPS.gameObject.SetActive(false);//it's like doing this sets it to false the next time your start the game? like it effects teh prefab
           //GameObject.Destroy(groundPoundPS.gameObject);//can't destroy?
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (force)
        {
            if (groundPoundPS && !particleHasPlayed)
            {
                Vector3 contactNormal = collision.contacts[0].normal;
                LayerMask colliderLayer = collision.collider.gameObject.layer;
                int groundLayer = 1 << 9;
                if (colliderLayer == (colliderLayer | (1 << groundLayer)))
                {
                    contactNormal += new Vector3(-90, 0, 0);
                }
                Quaternion rot = Quaternion.Euler(contactNormal);
                Instantiate(groundPoundPS, transform.position, rot);
                groundPoundPS.Play();
                particleHasPlayed = true;
                rb.AddForce(new Vector3(0, 30 * rb.velocity.y, 30 * rb.velocity.x));
            }
        }
        //don't collide with other arrows
        if (collision.gameObject && projectileParent)
        {
            if (!collision.gameObject.GetComponentInParent<Projectile>() && (projectileParent.elapsedTime > 0.1f))
            {
                //attempt to make arrows get stuck in target
                //issues may be caused by projectileparent sometimes being null?
                //looks weird, probably not worth doing
                //if ((collision.gameObject.GetComponent<AIControl>() || collision.gameObject.GetComponent<EnemyAIBody>()) && (projectileParent.elapsedTime > 0.15f))
                //{
                //    rb.isKinematic = true;
                //    rb.useGravity = false;
                //    projectileParent.speed = 0;
                //    projectileParent.transform.parent = collision.gameObject.transform;
                //}
                //Debug.Log(rb.velocity);
                //rb.velocity AddForce(new Vector3(0, 10 * velocity.y, 10 * velocity.x));
                projectileParent.isFlying = false;
                if (projectileParent.IsLeadBall)
                {
                    if (groundPoundPS && !particleHasPlayed)
                    {
                        Vector3 contactNormal = collision.contacts[0].normal;
                        LayerMask colliderLayer = collision.collider.gameObject.layer;
                        //this is to rotate the PS 90 degrees if hitting ground because for some reason normal doesn't work then, not sure needs some work
                        int groundLayer = 1 << 9;
                        if (colliderLayer == (colliderLayer | (1 << groundLayer)))
                        {
                            contactNormal += new Vector3(-90, 0, 0);
                        }
                        Quaternion rot = Quaternion.Euler(/*-90, 0, 0*/contactNormal);//instead make this normal to the surface it impacts
                        Instantiate(groundPoundPS, transform.position, rot);
                        //groundPoundPS.gameObject.SetActive(true);
                        groundPoundPS.Play();
                        particleHasPlayed = true;
                        rb.AddForce(new Vector3(0, 30 * rb.velocity.y, 30 * rb.velocity.x));
                    }
                }
            }
        }
    }
}
