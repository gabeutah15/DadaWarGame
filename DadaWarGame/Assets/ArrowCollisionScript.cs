using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowCollisionScript : MonoBehaviour
{
    Projectile projectileParent;
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        projectileParent = GetComponentInParent<Projectile>();//sometimes this is null but it shouldn't be?
        rb = GetComponent<Rigidbody>();
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
                projectileParent.isFlying = false;
            }
        }
    }
}
