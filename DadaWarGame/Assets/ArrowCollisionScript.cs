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
    // Start is called before the first frame update
    void Start()
    {
        projectileParent = GetComponentInParent<Projectile>();//sometimes this is null but it shouldn't be?
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (particleHasPlayed)
        {
            timeSinceImpact += Time.deltaTime;
        }

        if(timeSinceImpact > 2)
        {
            groundPoundPS.gameObject.SetActive(false);//cabn't destroy?
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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
                Debug.Log(rb.velocity);
                //rb.velocity AddForce(new Vector3(0, 10 * velocity.y, 10 * velocity.x));
                projectileParent.isFlying = false;
                if (projectileParent.IsLeadBall)
                {
                    if (groundPoundPS && !particleHasPlayed)
                    {
                        Quaternion rot = Quaternion.Euler(-90, 0, 0);
                        Instantiate(groundPoundPS, transform.position, rot);
                        groundPoundPS.Play();
                        particleHasPlayed = true;
                        rb.AddForce(new Vector3(0, 30 * rb.velocity.y, 30 * rb.velocity.x));
                    }
                }
            }
        }
    }
}
